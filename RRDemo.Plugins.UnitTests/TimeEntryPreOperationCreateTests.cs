using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RRDemo.Plugins.UnitTests
{
    [TestClass]
    public class TimeEntryPreOperationCreateTests
    {
        [TestMethod]
        public void Paste_Timeentry_In_The_Middle_Of_Date_Ranges_And_Change_Only_Current_Timeentry()
        {
            XrmFakedContext context = new XrmFakedContext();

            DateTime START = new DateTime(2020, 12, 1);
            DateTime END = new DateTime(2020, 12, 3, 12, 0, 0);

            //Populate virtual Dataverse with initial timeentries
            context.Initialize(new List<Entity> {
                new Entity(Constants.TIME_ENTRY)
                {
                    Id=Guid.NewGuid(),
                    Attributes =
                    {
                        [Constants.START] = START,
                        [Constants.END] = new DateTime(2020,12,1,12,0,0),
                        [Constants.DATE] = new DateTime(2020,12,1)
                    }
                },
                new Entity(Constants.TIME_ENTRY)
                {
                    Id=Guid.NewGuid(),
                    Attributes =
                    {
                        [Constants.START] = new DateTime(2020,12,3),
                        [Constants.END] = END,
                        [Constants.DATE] = new DateTime(2020,12,3)
                    }
                },
            });

            //Timeentry supposed to be created 
            var timeEntry = new Entity(Constants.TIME_ENTRY)
            {
                Id = Guid.NewGuid(),
                Attributes =
                    {
                        [Constants.START] = START,
                        [Constants.END] = END
                    }
            };

            //Run plugin
            TimeEntryPreOperationCreate plugin = new TimeEntryPreOperationCreate(string.Empty, string.Empty);
            context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 20);

            //Gathering results
            var query = new QueryExpression(Constants.TIME_ENTRY)
            {
                ColumnSet = new ColumnSet(new[] { Constants.START, Constants.END, Constants.DATE }),
                Criteria = new FilterExpression
                {
                    Conditions = {
                            new ConditionExpression(Constants.DATE,ConditionOperator.Between, new[]{ START.Date, END.Date })
                        }
                }
            };

            var result = context.GetOrganizationService().RetrieveMultiple(query).Entities.ToArray();

            //Check results
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.START), new DateTime(2020, 12, 2));
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.END), new DateTime(2020, 12, 3));
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.DATE), new DateTime(2020, 12, 2));
            Assert.AreEqual(timeEntry.GetAttributeValue<int>(Constants.DURATION), 1440);
        }

        [TestMethod]
        public void Paste_Timeentry_In_The_Beginning_Of_Date_Ranges_And_Add_Additional_Timeentry()
        {
            XrmFakedContext context = new XrmFakedContext();

            //Populate virtual Dataverse with initial timeentries
            DateTime START = new DateTime(2020, 12, 1);
            DateTime END = new DateTime(2020, 12, 3, 12, 0, 0);

            context.Initialize(new List<Entity> {
                new Entity(Constants.TIME_ENTRY)
                {
                    Id=Guid.NewGuid(),
                    Attributes =
                    {
                        [Constants.START] = new DateTime(2020,12,3),
                        [Constants.END] = END,
                        [Constants.DATE] = new DateTime(2020,12,3)
                    }
                },
            });

            //Timeentry supposed to be created 
            var timeEntry = new Entity(Constants.TIME_ENTRY)
            {
                Id = Guid.NewGuid(),
                Attributes =
                    {
                        [Constants.START] = START,
                        [Constants.END] = END
                    }
            };

            //Run plugin
            TimeEntryPreOperationCreate plugin = new TimeEntryPreOperationCreate(string.Empty, string.Empty);
            context.ExecutePluginWithTarget(plugin, timeEntry, "Create", 20);

            var query = new QueryExpression(Constants.TIME_ENTRY)
            {
                ColumnSet = new ColumnSet(new[] { Constants.START, Constants.END, Constants.DATE }),
                Criteria = new FilterExpression
                {
                    Conditions = {
                            new ConditionExpression(Constants.DATE,ConditionOperator.Between, new[]{ START.Date, END.Date })
                        }
                }
            };

            //Gathering results
            var result = context.GetOrganizationService().RetrieveMultiple(query).Entities.ToArray();

            //Check results
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.START), new DateTime(2020, 12, 1));
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.END), new DateTime(2020, 12, 2));
            Assert.AreEqual(timeEntry.GetAttributeValue<DateTime>(Constants.DATE), new DateTime(2020, 12, 1));
            Assert.AreEqual(timeEntry.GetAttributeValue<int>(Constants.DURATION), 1440);
        }
    }
}