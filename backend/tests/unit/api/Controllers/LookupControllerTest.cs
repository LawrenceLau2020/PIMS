using Xunit;
using System;
using Pims.Dal;
using Pims.Api.Controllers;
using Newtonsoft.Json;
using Moq;
using Model = Pims.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Entity = Pims.Dal.Entities;
using AutoMapper;
using Pims.Core.Test;
using Pims.Core.Comparers;

namespace Pims.Api.Test.Controllers
{
    [Trait("category", "unit")]
    [Trait("category", "api")]
    [Trait("group", "lookup")]
    public class LookupControllerTest
    {
        #region Variables
        #endregion

        #region Constructors
        public LookupControllerTest()
        {
        }
        #endregion

        #region Tests
        [Fact]
        public void GetAgencyCodes()
        {
            // Arrange
            var user = PrincipalHelper.CreateForRole("property-view");
            var helper = new TestHelper();
            var controller = helper.CreateController<LookupController>(user);

            var mapper = helper.GetService<IMapper>();
            var pimsService = helper.GetService<Mock<IPimsService>>();
            var agency = new Entity.Agency
            {
                Code = "MOH",
                Name = "Ministry of Health",
                Description = "The Ministry of Health"
            };
            pimsService.Setup(m => m.Lookup.GetAgencies()).Returns(new[] { agency });

            // Act
            var result = controller.GetAgencies();

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.CodeModel[]>(actionResult.Value);
            Assert.Equal(new[] { mapper.Map<Model.CodeModel>(agency) }, actualResult, new DeepPropertyCompare());
            pimsService.Verify(m => m.Lookup.GetAgencies(), Times.Once());
        }

        [Fact]
        public void GetPropertyClassificationCodes()
        {
            // Arrange
            var user = PrincipalHelper.CreateForRole("property-view");
            var helper = new TestHelper();
            var controller = helper.CreateController<LookupController>(user);

            var mapper = helper.GetService<IMapper>();
            var pimsService = helper.GetService<Mock<IPimsService>>();
            var propertyClassification = new Entity.PropertyClassification
            {
                Name = "Surplus Active",
            };
            pimsService.Setup(m => m.Lookup.GetPropertyClassifications()).Returns(new[] { propertyClassification });

            // Act
            var result = controller.GetPropertyClassifications();

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.CodeModel[]>(actionResult.Value);
            Assert.Equal(new[] { mapper.Map<Model.CodeModel>(propertyClassification) }, actualResult, new DeepPropertyCompare());
            pimsService.Verify(m => m.Lookup.GetPropertyClassifications(), Times.Once());
        }

        [Fact]
        public void GetRoleCodes()
        {
            // Arrange
            var user = PrincipalHelper.CreateForRole("property-view");
            var helper = new TestHelper();
            var controller = helper.CreateController<LookupController>(user);

            var mapper = helper.GetService<IMapper>();
            var pimsService = helper.GetService<Mock<IPimsService>>();
            var role = new Entity.Role
            {
                Id = Guid.NewGuid(),
                Name = "Ministry of Health",
                Description = "The Ministry of Health"
            };
            pimsService.Setup(m => m.Lookup.GetRoles()).Returns(new[] { role });

            // Act
            var result = controller.GetRoles();

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.CodeModel[]>(actionResult.Value);
            Assert.Equal(new[] { mapper.Map<Model.CodeModel>(role) }, actualResult, new DeepPropertyCompare());
            pimsService.Verify(m => m.Lookup.GetRoles(), Times.Once());
        }

        [Fact]
        public void GetAll()
        {
            // Arrange
            var user = PrincipalHelper.CreateForRole("property-view");
            var helper = new TestHelper();
            var controller = helper.CreateController<LookupController>(user);

            var mapper = helper.GetService<IMapper>();
            var pimsService = helper.GetService<Mock<IPimsService>>();
            var role = new Entity.Role
            {
                Id = Guid.NewGuid(),
                Name = "Ministry of Health",
                Description = "The Ministry of Health"
            };
            pimsService.Setup(m => m.Lookup.GetRoles()).Returns(new[] { role });

            var propertyClassification = new Entity.PropertyClassification
            {
                Name = "Surplus Active",
            };
            pimsService.Setup(m => m.Lookup.GetPropertyClassifications()).Returns(new[] { propertyClassification });

            var agency = new Entity.Agency
            {
                Code = "MOH",
                Name = "Ministry of Health",
                Description = "The Ministry of Health"
            };
            pimsService.Setup(m => m.Lookup.GetAgencies()).Returns(new[] { agency });

            // Act
            var agencyResult = controller.GetAgencies();
            var classificationResult = controller.GetPropertyClassifications();
            var roleResult = controller.GetRoles();
            var result = controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result); // TODO: Should not be testing all four functions.
            var agencyAction = Assert.IsType<JsonResult>(agencyResult);
            var roleAction = Assert.IsType<JsonResult>(roleResult);
            var classificationAction = Assert.IsType<JsonResult>(classificationResult);

            string allResult = JsonConvert.SerializeObject(actionResult.Value);
            string agenciesResult = JsonConvert.SerializeObject(agencyAction.Value);
            string rolesResult = JsonConvert.SerializeObject(roleAction.Value);
            string classificationsResult = JsonConvert.SerializeObject(classificationAction.Value);

            // Removing corresponding []'s as GetAll returns [{one,combined,list}]
            Assert.StartsWith(agenciesResult.Remove(agenciesResult.Length - 1, 1), allResult);
            Assert.Contains(classificationsResult[1..^1], allResult);
            Assert.EndsWith(rolesResult.Substring(1), allResult);
            pimsService.Verify(m => m.Lookup.GetAgencies(), Times.Exactly(2));
            pimsService.Verify(m => m.Lookup.GetPropertyClassifications(), Times.Exactly(2));
            pimsService.Verify(m => m.Lookup.GetRoles(), Times.Exactly(2));
        }
        #endregion
    }
}