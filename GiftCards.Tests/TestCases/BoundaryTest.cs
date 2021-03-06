﻿using GiftCards.BusinessLayer.Services;
using GiftCards.DataLayer;
using GiftCards.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GiftCards.Tests.TestCases
{
		
  public class BoundaryTest
    {

        private Mock<IMongoCollection<Buyer>> _mockCollection;
        private Mock<IMongoDBContext> _mockContext;
        private Buyer _buyer;
        private readonly IList<Buyer> _list;
        private Mock<IOptions<Mongosettings>> _mockOptions;

         static BoundaryTest()
        {
            if (!File.Exists("../../../../output_boundary_revised.txt"))
                try
                {
                    File.Create("../../../../output_boundary_revised.txt");
                }
                catch (Exception)
                {

                }
            else
            {
                File.Delete("../../../../output_boundary_revised.txt");
                File.Create("../../../../output_boundary_revised.txt");
            }
        }

        public BoundaryTest()
        {
           
            _buyer = new Buyer
            {
              FirstName="Buyer",
              LastName="buyer",
              PhoneNumber= 9087654321,
              Email = "buyer@gmail.com",
              Address = "Bangalore",
             
            };
            _mockCollection = new Mock<IMongoCollection<Buyer>>();
            _mockCollection.Object.InsertOne(_buyer);
            _mockContext = new Mock<IMongoDBContext>();
            //MongoSettings initialization
            _mockOptions = new Mock<IOptions<Mongosettings>>();
            _list = new List<Buyer>();
            _list.Add(_buyer);
        }
        Mongosettings settings = new Mongosettings()
        {
            Connection = "mongodb://user:password@127.0.0.1:27017/guestbook",
            DatabaseName = "guestbook"
        };

        [Fact]
        public async Task BoundaryTestFor_ValidBuyerName()
        {
            //mocking
            _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
            default(CancellationToken))).Returns(Task.CompletedTask);
            _mockContext.Setup(c => c.GetCollection<Buyer>(typeof(Buyer).Name)).Returns(_mockCollection.Object);

            //Craetion of new Db
            _mockOptions.Setup(s => s.Value).Returns(settings);
            var context = new MongoDBContext(_mockOptions.Object);
            var userRepo = new BuyerServices(context);

            //Act
            await userRepo.RegisterAsync(_buyer);
            var result = await userRepo.GetBuyerByIdAsync(_buyer.BuyerId);



            bool getisUserName = Regex.IsMatch(result.FirstName, @"^[a-zA-Z0-9]{4,10}$", RegexOptions.IgnoreCase);
            bool isUserName = Regex.IsMatch(_buyer.FirstName, @"^[a-zA-Z0-9]{4,10}$", RegexOptions.IgnoreCase);
            File.AppendAllText("../../../../output_boundary_revised.txt", "BoundaryTestFor_ValidBuyerName=" + isUserName.ToString() + "\n");
            //Assert
            Assert.True(isUserName);
            Assert.True(getisUserName);
        }
        [Fact]
        public async Task BoundaryTestFor_ValidBuyerPhoneNumberLength()
        {
            //mocking
            _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
            default(CancellationToken))).Returns(Task.CompletedTask);
            _mockContext.Setup(c => c.GetCollection<Buyer>(typeof(Buyer).Name)).Returns(_mockCollection.Object);

            //Craetion of new Db
            _mockOptions.Setup(s => s.Value).Returns(settings);
            var context = new MongoDBContext(_mockOptions.Object);
            var userRepo = new BuyerServices(context);

            //Act
            await userRepo.RegisterAsync(_buyer);
            var result = await userRepo.GetBuyerByIdAsync(_buyer.BuyerId);

            var MinLength = 10;
            var MaxLength = 10;

            //Action
            var actualLength = _buyer.PhoneNumber.ToString().Length;
            File.AppendAllText("../../../../output_boundary_revised.txt", "BoundaryTestFor_ValidBuyerPhoneNumberLength=" + actualLength + "\n");

            //Assert
            Assert.InRange(result.PhoneNumber.ToString().Length, MinLength, MaxLength);
            Assert.InRange(actualLength, MinLength, MaxLength);
        }

        [Fact]
        public async Task BoundaryTestFor_ValidBuyerEmail()
        {
            //mocking
            _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
            default(CancellationToken))).Returns(Task.CompletedTask);
            _mockContext.Setup(c => c.GetCollection<Buyer>(typeof(Buyer).Name)).Returns(_mockCollection.Object);

            //Craetion of new Db
            _mockOptions.Setup(s => s.Value).Returns(settings);
            var context = new MongoDBContext(_mockOptions.Object);
            var userRepo = new BuyerServices(context);

            //Act
            await userRepo.RegisterAsync(_buyer);
            var result = await userRepo.GetBuyerByIdAsync(_buyer.BuyerId);

            //Action
            bool CheckEmail = Regex.IsMatch(result.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            bool isEmail = Regex.IsMatch(_buyer.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            File.AppendAllText("../../../../output_boundary_revised.txt", "BoundaryTestFor_ValidBuyerEmail=" + isEmail.ToString()+"\n");

            //Assert
            Assert.True(isEmail);
            Assert.True(CheckEmail);
        }

        //[Fact]
        //public async Task BoundaryTestFor_ValidBuyerNameLength()
        //{
        //    //mocking
        //    _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
        //    default(CancellationToken))).Returns(Task.CompletedTask);
        //    _mockContext.Setup(c => c.GetCollection<Buyer>(typeof(Buyer).Name)).Returns(_mockCollection.Object);

        //    //Craetion of new Db
        //    _mockOptions.Setup(s => s.Value).Returns(settings);
        //    var context = new MongoDBContext(_mockOptions.Object);
        //    var userRepo = new BuyerServices(context);

        //    //Act
        //    await userRepo.RegisterAsync(_buyer);
        //    var result = await userRepo.GetBuyerByIdAsync(_buyer.BuyerId);

        //    var MinLength = 3;
        //    var MaxLength = 50;

        //    //Action
        //    var actualLength = _buyer.FirstName.Length;

        //    //Assert
        //    Assert.InRange(result.FirstName.Length, MinLength, MaxLength);
        //    Assert.InRange(actualLength, MinLength, MaxLength);
        //}


        //[Fact]
        //public async Task BoundaryTestFor_ValidBuyerId()
        //{
        //    //mocking
        //    _mockCollection.Setup(op => op.InsertOneAsync(_buyer, null,
        //    default(CancellationToken))).Returns(Task.CompletedTask);
        //    _mockContext.Setup(c => c.GetCollection<Buyer>(typeof(Buyer).Name)).Returns(_mockCollection.Object);

        //    //Craetion of new Db
        //    _mockOptions.Setup(s => s.Value).Returns(settings);
        //    var context = new MongoDBContext(_mockOptions.Object);
        //    var userRepo = new BuyerServices(context);

        //    //Act
        //    await userRepo.RegisterAsync(_buyer);
        //    var result = await userRepo.GetBuyerByIdAsync(_buyer.BuyerId);

        //    Assert.InRange(_buyer.BuyerId.Length, 20, 30);

        //}


    }
}
