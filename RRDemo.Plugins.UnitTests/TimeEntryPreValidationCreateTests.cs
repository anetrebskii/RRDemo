using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace RRDemo.Plugins.UnitTests
{
    [TestClass]
    public class TimeEntryPreValidationCreateTests
    {

        [TestMethod]
        public void Throw_Error_Duplicate_Timeentry()
        {
            XrmFakedContext context = new XrmFakedContext();

            //Populate virtual Dataverse with initial timeentries and initialize timeentry supposed to be created 
            Entity timeEntry = new Entity(Constants.TIME_ENTRY)
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    [Constants.START] = new DateTime(2020,12,1),
                    [Constants.END] = new DateTime(2020,12,1,12,0,0),
                    [Constants.DATE] = new DateTime(2020,12,1),
                    [Constants.RESOURCE] = new EntityReference(Constants.BOOKABLE_RESOURCE, Guid.NewGuid())
                }
            };

            context.Initialize(new List<Entity> { timeEntry });

            //Run plugin and gathering results
            TimeEntryPreValidationCreate plugin = new TimeEntryPreValidationCreate(string.Empty, string.Empty);

            Exception ex = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 10));

            //Check results
            const string EXPECTED_ERROR_MESSAGE = "Can't create duplicate time entry";

            Assert.AreEqual(EXPECTED_ERROR_MESSAGE, ex.Message);
        }

        [TestMethod]
        public void Throw_Error_If_Start_Equals_End()
        {
            XrmFakedContext context = new XrmFakedContext();

            //Initialize timeentry supposed to be created 
            DateTime SAME_DATE = new DateTime(2020, 12, 1, 12, 0, 0);

            Entity timeEntry = new Entity(Constants.TIME_ENTRY)
            {
                Attributes =
                {
                    [Constants.START] = SAME_DATE,
                    [Constants.END] = SAME_DATE,
                    [Constants.RESOURCE] = new EntityReference(Constants.BOOKABLE_RESOURCE, Guid.NewGuid())
                }
            };

            //Run plugin and gathering results
            TimeEntryPreValidationCreate plugin = new TimeEntryPreValidationCreate(string.Empty, string.Empty);

            Exception ex = Assert.ThrowsException<InvalidPluginExecutionException>(
                    () => context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 10));

            //Check results
            const string EXPECTED_ERROR_MESSAGE = "msdyn_start can't be equal msdyn_end";

            Assert.AreEqual(EXPECTED_ERROR_MESSAGE, ex.Message);
        }
    }
}