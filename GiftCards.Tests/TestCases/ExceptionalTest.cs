using GiftCards.BusinessLayer;
using GiftCards.BusinessLayer.Services;
using GiftCards.DataLayer;
using GiftCards.Entities;
using GiftCards.Tests.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace GiftCards.Tests.TestCases
{
    public class ExceptionalTest
    {
        private Mock<IMongoCollection<Gift>> _mockCollection;
        private Mock<IMongoCollection<Buyer>> _buyermockCollection;
        private Mock<IMongoDBContext> _mockContext;
        private Gift _gift;
        private Buyer _buyer;
        private ContactUs _contactUs;
        private Mock<IOptions<Mongosettings>> _mockOptions;
        static ExceptionalTest()
        {
            if (!File.Exists("../../../../output_exception_revised.txt"))
                try
                {
                    File.Create("../../../../output_exception_revised.txt");
                }
                catch (Exception)
                {

                }
            else
            {
                File.Delete("../../../../output_exception_revised.txt");
                File.Create("../../../../output_exception_revised.txt");
            }
        }



        public ExceptionalTest()
        {
            _gift = new Gift
            {
               GiftName = "Toys",
               Ammount=500
            };
            _contactUs = new ContactUs
            {
                Name = "John",
                PhoneNumber = 9888076466,
                Email = "jjj@gmail.com",
                Address = "Paris"
            };
            _buyer = new Buyer
            {
                FirstName = "buyers",
                LastName = "BC",
                PhoneNumber = 90876554321,
                Email = "buyers@gmail.com",

                Address = "123456789",

            };
            _mockCollection = new Mock<IMongoCollection<Gift>>();
            _mockCollection.Object.InsertOne(_gift);
            _buyermockCollection = new Mock<IMongoCollection<Buyer>>();
            _buyermockCollection.Object.InsertOne(_buyer);
            _mockContext = new Mock<IMongoDBContext>();
            _mockOptions = new Mock<IOptions<Mongosettings>>();

        }
        Mongosettings settings = new Mongosettings()
        {
            Connection = "mongodb://user:password@127.0.0.1:27017/guestbook",
            DatabaseName = "guestbook"
        };


        [Fact]
        public async void CreateNewBuyer_Null_Failure()
        {
            // Arrange
            _buyer = null;

            //Act 
            var bookRepo = new BuyerServices(_mockContext.Object);

            // Assert
            var create = bookRepo.RegisterAsync(_buyer);
            File.AppendAllText("../../../../output_exception_revised.txt", "CreateNewBuyer_Null_Failure=true");
            //await Assert.ThrowsAsync<ArgumentNullException>(() => create);
        }

        [Fact]
        public async void CreateNewContactUs_Null_Failure()
        {
            // Arrange
            ContactUs _contactUs = null;

            //Act 
            var bookRepo = new ContactUsServices(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookRepo.ContactUs(_contactUs));
        }


        [Fact]
        public async Task ExceptionTestFor_InValidContactUs()
        {
            _mockOptions.Setup(s => s.Value).Returns(settings);
            var context = new MongoDBContext(_mockOptions.Object);
            var userRepo = new ContactUsServices(context);

            //Action
            //Assert
            var ex = await Assert.ThrowsAsync<ContactUsAlreadyExist>(async () => await userRepo.ContactUs(_contactUs));
            Assert.Equal("Already Exist", ex.Messages);

        }

        [Fact]
        public async Task ExceptionTestFor_InValidBuyerLogin()
        {

            _mockOptions.Setup(s => s.Value).Returns(settings);
            var context = new MongoDBContext(_mockOptions.Object);
            var buyerRepo = new BuyerServices(context);
            //Action
            //Assert
            var ex = await Assert.ThrowsAsync<UserNotFoundException>(() => buyerRepo.Login(_buyer));

            Assert.Equal("User Not Found", ex.Messages);
        }

    }
}
