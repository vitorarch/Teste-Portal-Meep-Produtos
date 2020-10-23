using System;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using Xamarin.Forms;
using System.Runtime.InteropServices;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;

namespace SeleniumTests
{
    [TestClass]
    public class UntitledTestCase
    {
        private static IWebDriver driver;
        private StringBuilder verificationErrors;
        private static string baseURL;
        private bool acceptNextAlert = true;

        #region Class Iicializate
        [ClassInitialize]
        public static void InitializeClass(TestContext testContext) { }
    
        #endregion

        #region Test Inicialize & Test CleanUp
        [TestInitialize]
        public void InitializeTest()
        {
            verificationErrors = new StringBuilder();

            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://portal-estabelecimento-dev.azurewebsites.net/produtos/produto");
            // Login: samuel.antunes@promptus.com.br 
            driver.FindElement(By.Id("email")).Click();
            driver.FindElement(By.Id("email")).Clear();
            driver.FindElement(By.Id("email")).SendKeys("samuel.antunes@promptus.com.br");

            // Password: 123456
            driver.FindElement(By.Id("senha")).Click();
            driver.FindElement(By.Id("senha")).Clear();
            driver.FindElement(By.Id("senha")).SendKeys("123456");

            // Login button - click
            driver.FindElement(By.TagName("button")).Click();
            Thread.Sleep(2000);

            // Select tab Produtos
            driver.FindElements(By.ClassName("MuiListItemText-root"))[7].Click();
            Thread.Sleep(500);

            // Select sub item produto
            driver.FindElements(By.CssSelector("ul[class='MuiList-root sidebarListSubItem']"))[0].Click();

            //tentar fazer o login apenas uma vez, salvar e usar em todos os testes
            //string baseURL = driver.Url;
            //driver.Navigate().GoToUrl(baseURL);
            Thread.Sleep(2000);
            driver.SwitchTo().Frame(0);

        }

        [TestCleanup]
        public void CleanupTest()
        {
            Assert.AreEqual("", verificationErrors.ToString());
            driver.Close();
            driver.Dispose();
        }

        #endregion

        #region Grid Buttons

        [TestMethod]
        public void TestCase_GridButtons_SellectAllButton()
        {
            //Select the toggle button 'select all'
            driver.FindElement(By.CssSelector("span[class='lever']")).Click();
            Thread.Sleep(500);

            //Count how many buttons aren´t selected
            var productsSelectButton = driver.FindElements(By.CssSelector("input[class='ng-valid ng-dirty ng-valid-parse ng-touched ng-empty']"));

            Assert.IsTrue(productsSelectButton.Count.Equals(0));
        }

        [TestMethod]
        public void TestCase_GridButtons_EnableAllButton()
        {
            //Select the toggle button 'select all'
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//table[@id='table']/thead/tr/th[9]/div/label/span")).Click();
            //driver.FindElement(By.Id("chkHabilitarDesabilitarTodos")).Click();
            //driver.FindElement(By.CssSelector("input[ng-click='exibirMensagemConfirmacao()']")).Click();

            
            Thread.Sleep(500);

            driver.FindElement(By.CssSelector("button[ng-click='marcarOuDesmarcarTodos()']")).Click();

            //Count how many buttons aren't selected
            var productsEnableButton = driver.FindElements(By.CssSelector("input[ng-click='ativarOuDesativarProduto(produto)']"));
            int count = 0;

            foreach (var item in productsEnableButton)
                if (!item.Selected) count++;

            Assert.IsTrue(count.Equals(0));
        }

        [TestMethod]
        public void TestCase_GridButtons_DuplicateButton()
        {
            //Duplicate button - click
            driver.FindElement(By.CssSelector("button[ng-click='duplicarProduto(produto)']")).Click();
            Thread.Sleep(500);

            //Confirm duplicate
            driver.FindElement(By.Id("button-0")).Click();
            Thread.Sleep(2000);

            //Find all products name after duplicate
            var productsName = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findDuplicate = false;

            foreach (var x in productsName)
            {
                if (x.Text.Contains("Duplicado"));
                {
                    findDuplicate = true;
                    break;
                }
            }

            Assert.IsTrue(findDuplicate);
        }

        [TestMethod]
        public void TestCase_GridButtons_RemoveButton()
        {
            //Remove button - click
            string removedProduct = driver.FindElement(By.CssSelector("td[class='ng-binding']")).Text;
            driver.FindElement(By.CssSelector("button[ng-click='removerProduto(produto)']")).Click();
            Thread.Sleep(500);

            //Confirm remotion
            driver.FindElement(By.Id("button-0")).Click();
            Thread.Sleep(2000);

            //Test for any error when deleting
            bool removingErrorMessage = driver.FindElement(By.CssSelector("div[class='noty_message']")).Displayed;

            //Searching for removed product name after remotion
            var productsName = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            int count = 0;

            foreach (var x in productsName)
            {
                if (x.Text.Equals(removedProduct)) count++;
            }

            //Searching for removed product name after after refresh browser
            var productsNameAfterRefresh = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            int countAfterRefresh = 0;

            foreach (var x in productsNameAfterRefresh)
            {
                if (x.Text.Equals(removedProduct)) countAfterRefresh++;
            }

            Assert.IsTrue(count.Equals(0) && countAfterRefresh.Equals(0) && !removingErrorMessage);
        }

        [TestMethod]
        public void TestCase_GridButtons_EditButton()
        {
            Thread.Sleep(500);
            //Edit button - click
            driver.FindElement(By.XPath("//table[@id='table']/tbody/tr/td[10]/button/i")).Click();
            //driver.FindElement(By.CssSelector("button[ng-click='editarProduto(produto)']")).Click();
            Thread.Sleep(500);

            //Change product name
            driver.FindElement(By.Name("nome")).Click();
            driver.FindElement(By.Name("nome")).Clear();
            driver.FindElement(By.Name("nome")).SendKeys("Editando Nome");

            //Select a category
            driver.FindElement(By.Name("categoriaCliente")).Click();
            driver.FindElement(By.CssSelector("option[label='Balas']")).Click();

            //Enter the NCM
            driver.FindElement(By.Id("campoNcm")).Click();
            driver.FindElement(By.Id("campoNcm")).Clear();
            driver.FindElement(By.Id("campoNcm")).SendKeys("12345678");

            //Saving edits
            driver.FindElement(By.XPath("(//button[@type='submit'])[3]")).Click();
            Thread.Sleep(4000);

            //Searching edit product
            /////Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys("Editando");

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var productsOnTable = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool editProduct = false;

            foreach (var x in productsOnTable)
            {
                if (x.Text == "Editando Nome")
                {
                    editProduct = true;
                    break;
                }
            }

            Assert.IsTrue(editProduct);
        }

        #endregion

        #region Filters

        #region Product Name
        [TestMethod]
        public void TestCase_ProductFilter_Empty_Field_Complete_Table()
        {
            int notExpected = 0; //When filters fileds are empty the search should return complete table (items > 0)
            int numberItems = driver.FindElement(By.ClassName("body-table")).FindElements(By.TagName("tr")).Count;

            Assert.AreNotEqual(notExpected, numberItems);
        }

        [TestMethod]
        public void TestCase_ProductFilter_ProductName_Case_Sensitive()
        {
            //For this verification the prduct 'teste'and 'TESTE' were added

            /////Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys("teste");

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var teste = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findCaseSensitive = false;

            foreach (var x in teste)
            {
                if( x.Text == "TESTE")
                {
                    findCaseSensitive = true;
                    break;
                }
            }

            Assert.IsTrue(findCaseSensitive);
        }

        [TestMethod]
        public void TestCase_ProductFilter_ProductName_Incomplete_Words()
        {
            //For this verification the prduct 'teste'and 'TESTE' were added

            /////Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys("test"); //Expected to find products added 'test/Teste/TESTE'

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var teste = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findCorrespondentWord = false;

            foreach (var x in teste)
            {
                if (x.Text.ToLower() == "teste")
                {
                    findCorrespondentWord = true;
                    break;
                }
            }

            Assert.IsTrue(findCorrespondentWord);
        }

        [TestMethod]
        public void TestCase_ProductFilter_Word_With_Acent_Search_Words_Without_Acent()
        {
            //For this verification the prduct 'téste','TÉSTE' were added

            /////Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys("téste");

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var teste = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findCaseSensitive = false;

            foreach (var x in teste)
            {
                if (x.Text == "teste")
                {
                    findCaseSensitive = true;
                    break;
                }

            }

            Assert.IsTrue(findCaseSensitive);
        }

        [TestMethod]
        public void TestCase_ProductFilter_Word_Without_Acent_Search_Words_With_Acent()
        {
            //For this verification the prduct 'téste','TÉSTE' were added

            /////Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys("téste");

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var teste = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findCaseSensitive = false;

            foreach (var x in teste)
            {
                if (x.Text == "teste")
                {
                    findCaseSensitive = true;
                    break;
                }
            }
            Assert.IsTrue(findCaseSensitive);
        }
        #endregion

        #region Product Category

        [TestMethod]
        public void TestCase_ProductFilter_One_Category_Selected()
        {
            driver.FindElement(By.CssSelector("button[data-id='categoriasId']")).Click();
            driver.FindElements(By.CssSelector("a[role='option']"))[8].Click();
            string category = driver.FindElements(By.CssSelector("a[role='option']"))[8].Text;
            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var productsCategory = driver.FindElements(By.CssSelector("td[data-title='Categoria']"));
            int count = 0; 

            foreach (var x in productsCategory)
            {
                if (!(x.Text == category))
                {
                    count++;
                    break;
                }
            }

            Assert.IsTrue(count.Equals(0));

        }

        [TestMethod]
        public void TestCase_ProductFilter_Two_Category_Selected()
        {
            driver.FindElement(By.CssSelector("button[data-id='categoriasId']")).Click();
            driver.FindElements(By.CssSelector("a[role='option']"))[4].Click();
            string category = driver.FindElements(By.CssSelector("a[role='option']"))[4].Text;
            driver.FindElements(By.CssSelector("a[role='option']"))[8].Click();
            string category2 = driver.FindElements(By.CssSelector("a[role='option']"))[8].Text;
            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            var productsCategory = driver.FindElements(By.CssSelector("td[data-title='Categoria']"));
            int count = 0;

            foreach (var x in productsCategory)
            {
                if (!(x.Text == category) && !(x.Text == category2))
                {
                    count++;
                    break;
                }
            }

            Assert.IsTrue(count.Equals(0));

        }

        #endregion

        #endregion

        #region Add new product

        #region Page 01 - Category

        [TestMethod]
        public void TestCase_Add_Product_Page01_Without_Select_Category()
        {

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Error message
            bool error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;

            Assert.IsTrue(error);
        }

        #endregion

        #region Page 02 - Product
        [TestMethod]
        public void TestCase_Add_Product_Page02_Without_Enter_All_Fields()
        {
            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(string.Empty);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(string.Empty);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);
            
            //Error message
            bool error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;

            Assert.IsTrue(error);
        }

        [TestMethod]
        public void TestCase_Add_Product_Page02_Without_Enter_ProductValue()
        {
            string productName = CreateRandomProductName();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(string.Empty);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Error message
            bool error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;

            Assert.IsTrue(error);

        }

        [TestMethod]
        public void TestCase_Add_Product_Page02_Without_Enter_ProductName()
        {
            string productValue = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(string.Empty);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productValue);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Error message
            bool error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;

            Assert.IsTrue(error);
        }

        #endregion

        #region Page 03 - Additionals

        [TestMethod]
        public void TestCase_Add_Product_Page03_Trying_Add_Addicional_Without_Select_One()
        {
            string productName = CreateRandomProductName();
            string productQuantity = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productQuantity);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 03 - Additonals
            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys(productQuantity);

            //------------------Trying add additional without select one ----------------------------

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            //Error message
            bool error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;

            Assert.IsTrue(error);
        }

        [TestMethod]
        public void TestCase_Add_Product_Page03_Verifying_Correct_Addition_Of_Additionals()
        {
            //Creating random name and value for product
            string productName = CreateRandomProductName();
            string productValue = CreateRandomProductValue();
            string productQuantity = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productValue);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys(productQuantity);

            //------------------ Add first aditional ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("combo-box-demo-option-2")).Click();

            //Select aditional type already selected

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys(productQuantity);

            //Allowed free aditional
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Limit of free aditional quantity
            driver.FindElements(By.Name("freeQuantity"))[1].Click();
            driver.FindElements(By.Name("freeQuantity"))[1].SendKeys(productQuantity);

            //Add aditional product an observation
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma observação para o produto adicional");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            bool firstAditionalAdd = driver.FindElement(By.CssSelector("tr[class='MuiTableRow-root']")).Displayed;

            //------------------ Add second aditional ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("combo-box-demo-option-5")).Click();

            //Select aditional type already selected

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys(productQuantity);

            //Allowed free aditional
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Limit of free aditional quantity
            driver.FindElements(By.Name("freeQuantity"))[1].Click();
            driver.FindElements(By.Name("freeQuantity"))[1].SendKeys(productQuantity);

            //Add aditional product an observation
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma observação para o produto adicional");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            bool secondAditionalAdd = driver.FindElements(By.CssSelector("tr[class='MuiTableRow-root']"))[1].Displayed;

            int ProductsOnGrid = driver.FindElements(By.CssSelector("tr[class='MuiTableRow-root']")).Count;

            int ProductsInfo = driver.FindElements(By.CssSelector("th[class='MuiTableCell-root MuiTableCell-body']")).Count;

            bool ok = firstAditionalAdd == true && secondAditionalAdd == true && ProductsOnGrid.Equals(2) && ProductsInfo.Equals(8);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void TestCase_Add_Product_Page03_Entering_Negative_Quantity()
        {
            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys("teste");

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys("12");

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 03 - Additonals
            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys("-10");

            //Advance button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[1].Click();

            //Error message
            bool error;
            try
            {
                error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;
            } catch (OpenQA.Selenium.NotFoundException e)
            {
                error = false;
            }

            Assert.IsTrue(error);

        }

        [TestMethod]
        public void TestCase_Add_Product_Page02_Without_Entering_Negative_Quantity_For_Additionals()
        {
            string productName = CreateRandomProductName();
            string productQuantity = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productQuantity);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 03 - Additonals
            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys(productQuantity);

            //------------------ Add aditional with negative quantity ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("combo-box-demo-option-2")).Click();

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys("-10");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            //Advance button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[1].Click();
            Thread.Sleep(1000);

            //Error message
            bool error;
            try
            {
                error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                error = false;
            }

            Assert.IsTrue(error);
        }

        [TestMethod]
        public void TestCase_Add_Product_Page02_Without_Entering_Negative_Quantity_For_Free_Aditionals()
        {
            string productValue = CreateRandomProductValue();

            string productName = CreateRandomProductName();
            string productQuantity = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            Thread.Sleep(1000);
            driver.FindElement(By.Id(id + "-option-5")).Click();

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productQuantity);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 03 - Additonals
            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys(productQuantity);

            //------------------ Add first aditional ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("combo-box-demo-option-2")).Click();

            //Select aditional type already selected

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys(productQuantity);

            //Allowed free aditional
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Limit of free aditional quantity
            driver.FindElements(By.Name("freeQuantity"))[1].Click();
            driver.FindElements(By.Name("freeQuantity"))[1].SendKeys("-10");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            //Advance button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[1].Click();
            Thread.Sleep(1000);

            //Error message
            bool error;
            try
            {
                error = driver.FindElement(By.CssSelector("div[role='alert']")).Displayed;
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                error = false;
            }

            Assert.IsTrue(error);
        }

        #endregion

        #region Add Succeswsefull

        [TestMethod]
        public void TestCase_Add_Product_Successefull()
        {
            //Creating random name and value for product
            string productName = CreateRandomProductName();
            string productValue = CreateRandomProductValue();
            string productQuantity = CreateRandomProductValue();

            //Add button - click
            driver.FindElement(By.CssSelector("button[class='btn btn-primary adicionarCrud']")).Click();
            Thread.Sleep(1000);

            //Page 01 - Category
            //Selecting product category
            driver.FindElements(By.TagName("input"))[1].Click();
            string id = driver.FindElements(By.TagName("input"))[1].GetAttribute("id");
            driver.FindElement(By.Id(id + "-option-6")).Click();
            //Thread.Sleep(1000);
            //string productCategory = driver.FindElement(By.Id(id + "-option-5")).Text;

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 02 - Product
            //Add product name
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[0].SendKeys(productName);

            //Add product value
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].Click();
            driver.FindElements(By.CssSelector("input[class='MuiInputBase-input MuiOutlinedInput-input']"))[1].SendKeys(productValue);

            //Check older than 18
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Add prduct description
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma descrição");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Page 03 - Additonals
            //Enter limit for free additional
            driver.FindElement(By.Name("freeQuantity")).Click();
            driver.FindElement(By.Name("freeQuantity")).SendKeys(productQuantity);

            //------------------ Add first aditional ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click(); ;
            driver.FindElement(By.Id("combo-box-demo-option-2")).Click();

            //Select aditional type already selected

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys(productQuantity);

            //Allowed free aditional
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Limit of free aditional quantity
            driver.FindElements(By.Name("freeQuantity"))[1].Click();
            driver.FindElements(By.Name("freeQuantity"))[1].SendKeys(productQuantity);

            //Add aditional product an observation
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma observação para o produto adicional");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            //------------------ Add second aditional ----------------------------

            //Add aditional product
            driver.FindElements(By.Id("combo-box-demo"))[1].Click();
            driver.FindElement(By.Id("combo-box-demo-option-5")).Click();

            //Select aditional type already selected

            //Enter aditional quantity
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].Click();
            driver.FindElements(By.CssSelector("input[name='quantidade']"))[0].SendKeys(productQuantity);

            //Allowed free aditional
            driver.FindElement(By.CssSelector("input[class='jss185 MuiSwitch-input']")).Click();

            //Limit of free aditional quantity
            driver.FindElements(By.Name("freeQuantity"))[1].Click();
            driver.FindElements(By.Name("freeQuantity"))[1].SendKeys(productQuantity);

            //Add aditional product an observation
            driver.FindElement(By.TagName("textarea")).Click();
            driver.FindElement(By.TagName("textarea")).SendKeys("testando uma observação para o produto adicional");

            //Add aditional button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[0].Click();
            Thread.Sleep(500);

            //Advance button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[1].Click();

            //Page 04 - Observations
            //Advance button
            driver.FindElements(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']"))[1].Click();

            //Page 05 - Fiscal
            //Enter NCM number
            driver.FindElement(By.CssSelector("input[class='MuiInputBase-input MuiInput-input']")).Click();
            driver.FindElement(By.CssSelector("input[class='MuiInputBase-input MuiInput-input']")).Clear();
            driver.FindElement(By.CssSelector("input[class='MuiInputBase-input MuiInput-input']")).SendKeys("12345678");

            //Advance button
            driver.FindElement(By.CssSelector("button[class='MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary']")).Click();
            Thread.Sleep(1000);

            //Register Successfull
            //Enable product for sale
            driver.FindElement(By.CssSelector("input[class='jss192 MuiSwitch-input']")).Click();

            //Exit button
            driver.FindElement(By.CssSelector("a[href='/produtos/produto']")).Click();

            // ------------- Verifying if product was correctly on table -------------------------------

            //Enter the frmae
            driver.SwitchTo().Frame(0);

            //Product name
            driver.FindElement(By.Id("nomeProduto")).Click();
            driver.FindElement(By.Id("nomeProduto")).SendKeys(productName);
            Thread.Sleep(100);

            //Button Search - click action
            driver.FindElement(By.CssSelector("button[class='btn botao-busca ng-binding']")).Click();

            //Load table
            Thread.Sleep(2000);

            //Serching the product in the results
            var teste = driver.FindElements(By.CssSelector("td[class='ng-binding']"));
            bool findProductAdded = false;

            foreach (var x in teste)
            {
                if (x.Text == productName)
                {
                    findProductAdded = true;
                    break;
                }
            }

            Assert.IsTrue(findProductAdded);
        }

        #endregion

        #endregion

        #region Auxliar Functions

        public static string CreateRandomProductName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 7)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static string CreateRandomProductValue()
        {
            Random rnd = new Random();
            return rnd.Next(5, 100).ToString();
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }

        #endregion
    }
}