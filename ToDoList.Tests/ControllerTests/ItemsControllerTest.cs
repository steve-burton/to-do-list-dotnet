﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Controllers;
using ToDoList.Models;
using Xunit;
using Moq;
using ToDoList.Tests.Models;

namespace ToDoList.Tests
{
    public class ItemsControllerTest : IDisposable
    {
        public Mock<IItemRepository> mock = new Mock<IItemRepository>();

        EFItemRepository db = new EFItemRepository(new TestDbContext());
        public void MakeTestDB()
        {
            mock.Setup(m => m.Items).Returns(new Item[]
            {
                new Item { ItemId = 1, Description = "wash dog"},
                new Item { ItemId =2, Description = "do dishes" },
                new Item { ItemId = 3, Description = "sweep"}
            }.AsQueryable());
        }
        [Fact]
        public void Mock_GetViewResultIndex_Test() //Confirms route returns view
        {
            //Arrange
            MakeTestDB();
            ItemsController controller = new ItemsController(mock.Object);

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Mock_IndexListOfItems_Test() //Confirms model as list of items
        {
            // Arrange
            MakeTestDB();
            ViewResult indexView = new ItemsController(mock.Object).Index() as ViewResult;

            // Act
            var result = indexView.ViewData.Model;

            // Assert
            Assert.IsType<List<Item>>(result);
        }

        [Fact]
        public void Mock_ConfirmEntry_Test() //Confirms presence of known entry
        {
            // Arrange
            MakeTestDB();
            ItemsController controller = new ItemsController(mock.Object);
            Item testItem = new Item();
            testItem.Description = "Wash the dog";
            testItem.ItemId = 1;

            // Act
            ViewResult indexView = controller.Index() as ViewResult;
            var collection = indexView.ViewData.Model as IEnumerable<Item>;

            // Assert
            Assert.Contains<Item>(testItem, collection);
        }

        [Fact]
        public void DB_CreateNewEntry_Test()
        {
            // Arrange
            ItemsController controller = new ItemsController(db);
            Item testItem = new Item();
            testItem.Description = "TestDb Item";

            // Act
            controller.Create(testItem);
            var collection = (controller.Index() as ViewResult).ViewData.Model as IEnumerable<Item>;

            // Assert
            Assert.Contains<Item>(testItem, collection);
        }
        public void Dispose()
        {
            db.DeleteAll();
        }
    }
}