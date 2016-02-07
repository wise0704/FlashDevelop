using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Settings;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet.Enums;

// NOTE: Can be cleaned up. Fast tests just to check everything works correctly!

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASDocumentationTests
    {
        private MainForm mainForm;
        private ISettings settings;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            mainForm = new MainForm();
            settings = Substitute.For<ISettings>();
            settings.UseTabs = true;
            settings.IndentSize = 4;
            settings.SmartIndentType = SmartIndent.CPP;
            settings.TabIndents = true;
            settings.TabWidth = 4;
            mainForm.Settings = settings;
            mainForm.StandaloneMode = false;
            PluginBase.Initialize(mainForm);
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            settings = null;
            mainForm.Dispose();
            mainForm = null;
        }

        public static IEnumerable<TestCaseData> GetNewLinesTestCases
        {
            get
            {
                yield return new TestCaseData("<div>Simple text</div>", "<div>Simple text</div>", 2).SetName("SimpleCase");
                yield return new TestCaseData("<div>Multiline<br> text<br/>empty</div>", "<div>Multiline<br> text</div>", 2).SetName("SimpleMultiLine");
                yield return new TestCaseData("<div>Multiline<p>text</p><br/><p>empty</p></div>", "<div>Multiline<p>text</p></div>", 2).SetName("MultiLineWithParagraphs");
                yield return new TestCaseData("<div>Multiline<p>text</p><br/><p>empty</p></div>", "<div>Multiline<p>text</p><br/><p>empty</p></div>", 3).SetName("MultiLineWithParagraphsAndLineBreaks");
                yield return new TestCaseData("<p>Multiline</p><p>text</p><br/><p>empty</p>", "<p>Multiline</p><p>text</p>", 2).SetName("MultiLineParagraphs");
                yield return new TestCaseData("<pre>Multiline\r\ntext\r\nempty</pre>", "<pre>Multiline\r\ntext</pre>", 2).SetName("FormattedBlock");
                yield return new TestCaseData("Sample:<pre>Multiline\r\ntext\r\nempty</pre>", "Sample:<pre>Multiline</pre>", 2).SetName("FormattedBlockWithTextBefore");
                yield return new TestCaseData("The Event class is\n\n <p>The properties of the Event <ph>Events</ph>class</p><p>The methods of the Event</p>", "The Event class is\n\n <p>The properties of the Event <ph>Events</ph>class</p>", 2).SetName("EventExtract");
            }
        }

        [Test]
        [TestCaseSource("GetNewLinesTestCases")]
        public void GetNLines(string source, string result, int lineCount)
        {
            var parsedLines = ASDocumentation.GetNLinesOf(source, lineCount, false);

            Assert.AreEqual(result, parsedLines);
        }

        [Test]
        public void GetTipFullDetails_HaxeWithParams()
        {
            var member = new Model.MemberModel
            {
                Comments = @"
          test
          @param arg1 asd
          @param arg2 c
          @return
         "
            };

            var pluginMain = Substitute.For<PluginMain>();
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            ASContext.GlobalInit(pluginMain);
            ASContext.CommonSettings.Returns(new GeneralSettings {SmartTipsEnabled = true});

            var cb = ASDocumentation.GetTipFullDetails(member, null);

            
        }
    }
}
