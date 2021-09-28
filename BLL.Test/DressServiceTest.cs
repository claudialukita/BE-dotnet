using BLL.Kafka;
using BLL.Messaging;
using BLL.Redis;
using BLL.Test.Common;
using DAL.Model;
using DAL.Repositories;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BLL.Test
{
    public class DressServiceTest
    {
        private IEnumerable<DressModel> dresses;
        private Mock<IRedisService> redis;
        private Mock<IUnitOfWork> uow;
        private Mock<IKafkaSender> kafka;


        public DressServiceTest()
        {
            dresses = CommonHelper.LoadDataFromFile<IEnumerable<DressModel>>(@"..\..\..\MockData\Dress.json");
            //dresses = CommonHelper.LoadDataFromFile<IEnumerable<DressModel>>(@"MockData\Dress.json");
            uow = MockUnitOfWork();
            redis = MockRedis();
            kafka = MockKafka();
        }

        private DressService CreateDressService()
        {
            return new DressService(kafka.Object, uow.Object, redis.Object);
        }

        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var dressModelQueryable = dresses.AsQueryable().BuildMock().Object;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(su => su.DressRepository.GetAll())
                .Returns(dressModelQueryable);

            mockUnitOfWork
                .Setup(su => su.DressRepository.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => dressModelQueryable.FirstOrDefault());

            return mockUnitOfWork;

        }

        private Mock<IRedisService> MockRedis()
        {
            var mockRedis = new Mock<IRedisService>();
            
            mockRedis
                .Setup(x => x.GetAsync<DressModel>(It.Is<string>(x => x.Equals("dress_dressId:6c86861c-ba5e-4507-9cdf-48053885f6bd"))))
                .ReturnsAsync(dresses.FirstOrDefault(x => x.Id == Guid.Parse("6c86861c-ba5e-4507-9cdf-48053885f6bd")))
                .Verifiable();

            mockRedis
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockRedis
              .Setup(x => x.DeleteAsync(It.IsAny<string>())).Verifiable();

            return mockRedis;
        }

        private Mock<IKafkaSender> MockKafka()
        {

            var mockKafka = new Mock<IKafkaSender>();

            mockKafka
                .Setup(k => k.SendAsync("testTopic", "mgs"))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockKafka;
        }

        [Fact]
        public async Task GetAllAsync_Success()
        {
            var expected = dresses;

            var svc = CreateDressService();

            var actual = await svc.GetAllDressAsync();

            actual.Should().BeEquivalentTo(expected);

        }

        [Theory]
        [InlineData("68F8A912-55A9-4267-9975-08072B2F5DB6")]
        [InlineData("6C86861C-BA5E-4507-9CDF-48053885F6BD")]
        public async Task GetDressById_Success(string idBody)
        {
            //arrange
            var id = Guid.Parse(idBody);

            var expected = dresses.First(x => x.Id == id);

            var svc = CreateDressService();

            //act
            var actual = await svc.GetDressIdAsync(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("68F8A912-55A9-4267-9975-08072B2F5DB5")]
        public async Task GetDressById_NotFound(string idBody)
        {
            //arrange
            var id = Guid.Parse(idBody);

            var svc = CreateDressService();

            //act
            Func<Task> act = async () => { await svc.GetDressIdAsync(id); };

            //assert
            await act.Should().ThrowAsync<Exception>();

        }

        [Theory]
        [InlineData("6c86861c-ba5e-4507-9cdf-48053885f6bd")]
        public async Task GetDressById_InRedis_Success(string idBody)
        {

            //arrange
            var id = Guid.Parse(idBody);
            var expected = dresses.First(x => x.Id == id);

            var svc = CreateDressService();

            //act
            var actual = await svc.GetDressIdAsync(id);

            //assert
            actual.Should().BeEquivalentTo(expected);

            redis.Verify(x => x.GetAsync<DressModel>($"dress_dressId:{idBody}"), Times.Once);
            redis.Verify(x => x.SaveAsync($"dress_dressId::{idBody}", It.IsAny<DressModel>()), Times.Never);
        }

        //[Theory]
        ////[InlineData("66e5454e-1f51-40da-8a7c-4a58bde59396")]
        //[InlineData("f8d7fa0d-84ff-416a-acf7-69c358f8b281")]

        //public async Task GetDressById_NotInRedis_Success(string idBody)
        //{

        //    //arrange
        //    var id = Guid.Parse(idBody);
        //    var expected = dresses.First(x => x.Id == id);

        //    var svc = CreateDressService();

        //    //act
        //    var actual = await svc.GetDressIdAsync(id);

        //    //assert
        //    actual.Should().BeEquivalentTo(expected);

        //    redis.Verify(x => x.GetAsync<DressModel>($"dress_dressId:{idBody}"), Times.Once);
        //    redis.Verify(x => x.SaveAsync($"dress_dressId::{idBody}", It.IsAny<DressModel>()), Times.Once);
        //}


    }
}
