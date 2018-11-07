﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Reddit.NET;
using Reddit.NET.Models.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reddit.NETTests.ModelTests
{
    [TestClass]
    public class WidgetsTests : BaseTests
    {
        [TestMethod]
        public void Workflow()
        {
            Dictionary<string, string> testData = GetData();
            RedditAPI reddit = new RedditAPI(testData["AppId"], testData["RefreshToken"]);

            // Create TextArea Widget.  --Kris
            WidgetTextArea widgetTextArea = reddit.Models.Widgets.Add(new WidgetTextArea("Test Widget", "This is a test."), testData["Subreddit"]);

            // Create Calendar Widget.  --Kris
            WidgetCalendar widgetCalendar = reddit.Models.Widgets.Add(new WidgetCalendar(
                new WidgetCalendarConfiguration(0, true, true, true, true, true), "kris.craig@gmail.com",
                false, "Test Calendar Widget", new WidgetStyles()), testData["Subreddit"]);

            // Create CommunityList Widget.  --Kris
            WidgetCommunityListDetailed widgetCommunityList = reddit.Models.Widgets.Add(
                    new WidgetCommunityList(new List<string> { testData["Subreddit"], "RedditDotNETBot" },
                    "Test CommunityList Widget", new WidgetStyles()), testData["Subreddit"]);

            Assert.IsNotNull(widgetTextArea);
            Assert.IsNotNull(widgetCalendar);
            Assert.IsNotNull(widgetCommunityList);

            // Retrieve the widgets we just created.  --Kris
            WidgetResults widgetResults = reddit.Models.Widgets.Get(false, "RedditDotNETBot");

            Assert.IsNotNull(widgetResults);
            Assert.IsNotNull(widgetResults.Items);

            // Figure out which result goes with which type (only one of each which makes things easier).  --Kris
            string widgetTextAreaId = null;
            string widgetCalendarId = null;
            string widgetCommunityListId = null;
            foreach (KeyValuePair<string, dynamic> pair in widgetResults.Items)
            {
                JObject data = pair.Value;
                if (data.ContainsKey("kind") && data.ContainsKey("id"))
                {
                    switch (data["kind"].ToString())
                    {
                        case "textarea":
                            widgetTextAreaId = data["id"].ToString();
                            break;
                        case "calendar":
                            widgetCalendarId = data["id"].ToString();
                            break;
                        case "community-list":
                            widgetCommunityListId = data["id"].ToString();
                            break;
                    }
                }
            }

            Assert.IsNotNull(widgetTextAreaId);
            Assert.IsNotNull(widgetCalendarId);
            Assert.IsNotNull(widgetCommunityListId);

            // Modify the TextArea Widget.  --Kris
            WidgetTextArea widgetTextAreaUpdated = reddit.Models.Widgets.Update(widgetTextAreaId, new WidgetTextArea("Test Widget B", "This is a test."), 
                testData["Subreddit"]);

            Assert.IsNotNull(widgetTextAreaUpdated);

            // Modify the Calendar Widget.  --Kris
            WidgetCalendar widgetCalendarUpdated = reddit.Models.Widgets.Update(widgetCalendarId, new WidgetCalendar(
                new WidgetCalendarConfiguration(0, true, true, true, true, true), "kris.craig@gmail.com",
                false, "Test Calendar Widget B", new WidgetStyles()), testData["Subreddit"]);

            Assert.IsNotNull(widgetCalendarUpdated);

            // Modify the CommunityList Widget.  --Kris
            WidgetCommunityList widgetCommunityListUpdated = reddit.Models.Widgets.Update(widgetCommunityListId,
                    new WidgetCommunityList(new List<string> { testData["Subreddit"], "RedditDotNETBot" },
                    "Test CommunityList Widget", new WidgetStyles()), testData["Subreddit"]);

            Assert.IsNotNull(widgetCommunityListUpdated);

            // Reverse the order of widgets in the test subreddit.  --Kris
            List<string> order = widgetResults.Layout.Sidebar.Order;
            order.Reverse();

            reddit.Models.Widgets.UpdateOrder("sidebar", order, testData["Subreddit"]);

            // Delete the widgets.  --Kris
            reddit.Models.Widgets.Delete(widgetTextAreaId, testData["Subreddit"]);
            reddit.Models.Widgets.Delete(widgetCalendarId, testData["Subreddit"]);
            reddit.Models.Widgets.Delete(widgetCommunityListId, testData["Subreddit"]);
        }
    }
}