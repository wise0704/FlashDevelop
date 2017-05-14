using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using ProjectManager.Controls;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Utilities;
using System.Collections.Generic;
using ProjectManager.Projects;
using PluginCore.Helpers;
using PluginCore.Controls;

namespace ProjectManager.Controls
{
    public class FDMenus
    {
        public ToolStripMenuItem View;
        public ToolStripMenuItem GlobalClasspaths;
        public ToolStripButton TestMovie;
        public ToolStripButton BuildProject;
        public ToolStripComboBoxEx ConfigurationSelector;
        public ToolStripComboBoxEx TargetBuildSelector;
        public RecentProjectsMenu RecentProjects;
        public ProjectMenu ProjectMenu;

        public FDMenus(IMainForm mainForm)
        {
            // modify the file menu
            ToolStripMenuItem fileMenu = (ToolStripMenuItem)mainForm.FindMenuItem("FileMenu");
            RecentProjects = new RecentProjectsMenu();
            fileMenu.DropDownItems.Insert(5, RecentProjects);

            // modify the view menu
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)mainForm.FindMenuItem("ViewMenu");
            View = new ToolStripMenuItem(TextHelper.GetString("Label.MainMenuItem"));
            View.Image = Icons.Project.Img;
            viewMenu.DropDownItems.Add(View);
            PluginBase.MainForm.RegisterShortcut("View.ShowProject", View);

            // modify the tools menu - add a nice GUI classpath editor
            ToolStripMenuItem toolsMenu = (ToolStripMenuItem)mainForm.FindMenuItem("ToolsMenu");
            GlobalClasspaths = new ToolStripMenuItem(TextHelper.GetString("Label.GlobalClasspaths"));
            GlobalClasspaths.Image = Icons.Classpath.Img;
            toolsMenu.DropDownItems.Insert(toolsMenu.DropDownItems.Count - 4, GlobalClasspaths);
            PluginBase.MainForm.RegisterShortcut("Tools.GlobalClasspaths", Keys.Control | Keys.F9, GlobalClasspaths);

            ProjectMenu = new ProjectMenu();

            MenuStrip mainMenu = mainForm.MenuStrip;
            mainMenu.Items.Insert(5, ProjectMenu);

            ToolStrip toolBar = mainForm.ToolStrip;
            toolBar.Items.Add(new ToolStripSeparator());

            toolBar.Items.Add(RecentProjects.ToolbarSelector);

            BuildProject = new ToolStripButton(Icons.Gear.Img);
            BuildProject.Name = "BuildProject";
            BuildProject.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.BuildProject");
            PluginBase.MainForm.RegisterShortcut("Project.BuildProject", BuildProject);
            toolBar.Items.Add(BuildProject);

            TestMovie = new ToolStripButton(Icons.GreenCheck.Img);
            TestMovie.Name = "TestMovie";
            TestMovie.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.TestMovie");
            PluginBase.MainForm.RegisterShortcut("Project.TestMovie", TestMovie);
            toolBar.Items.Add(TestMovie);

            ConfigurationSelector = new ToolStripComboBoxEx();
            ConfigurationSelector.Name = "ConfigurationSelector";
            ConfigurationSelector.ToolTipText = TextHelper.GetString("ToolTip.SelectConfiguration");
            ConfigurationSelector.Items.AddRange(new string[] { TextHelper.GetString("Info.Debug"), TextHelper.GetString("Info.Release") });
            ConfigurationSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigurationSelector.AutoSize = false;
            ConfigurationSelector.Enabled = false;
            ConfigurationSelector.Width = ScaleHelper.Scale(GetThemeWidth("ProjectManager.TargetBuildSelectorWidth", 85));
            ConfigurationSelector.Margin = new Padding(1, 0, 0, 0);
            ConfigurationSelector.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            ConfigurationSelector.Font = PluginBase.Settings.DefaultFont;
            toolBar.Items.Add(ConfigurationSelector);
            PluginBase.MainForm.RegisterShortcut("Project.ConfigurationSelectorToggle", Keys.Control | Keys.F5, ConfigurationSelector);

            TargetBuildSelector = new ToolStripComboBoxEx();
            TargetBuildSelector.Name = "TargetBuildSelector";
            TargetBuildSelector.ToolTipText = TextHelper.GetString("ToolTip.TargetBuild");
            TargetBuildSelector.AutoSize = false;
            TargetBuildSelector.Width = ScaleHelper.Scale(GetThemeWidth("ProjectManager.ConfigurationSelectorWidth", 120));
            TargetBuildSelector.Margin = new Padding(1, 0, 0, 0);
            TargetBuildSelector.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            TargetBuildSelector.Font = PluginBase.Settings.DefaultFont;
            toolBar.Items.Add(TargetBuildSelector);
            PluginBase.MainForm.RegisterShortcut("Project.TargetBuildSelector", Keys.Control | Keys.F7, TargetBuildSelector);
            EnableTargetBuildSelector(false);
        }

        private int GetThemeWidth(string themeId, int defaultValue)
        {
            string strValue = PluginBase.MainForm.GetThemeValue(themeId);
            int intValue;
            if (int.TryParse(strValue, out intValue)) return intValue;
            else return defaultValue;
        }

        public void EnableTargetBuildSelector(bool enabled)
        {
            var target = TargetBuildSelector.Text; // prevent occasional loss of value when the control is disabled
            TargetBuildSelector.Enabled = enabled;
            TargetBuildSelector.Text = target;
        }

        public bool DisabledForBuild
        {
            get { return !TestMovie.Enabled; }
            set
            {
                BuildProject.Enabled = TestMovie.Enabled = ProjectMenu.ProjectItemsEnabledForBuild = ConfigurationSelector.Enabled = !value;
                EnableTargetBuildSelector(!value);
            }
        }

        public void SetProject(Project project)
        {
            RecentProjects.AddOpenedProject(project.ProjectPath);
            ConfigurationSelector.Enabled = true;
            ProjectMenu.ProjectItemsEnabled = true;
            TestMovie.Enabled = true;
            BuildProject.Enabled = true;
            ProjectChanged(project);
        }

        public void CloseProject()
        {
            TargetBuildSelector.Text = "";
            EnableTargetBuildSelector(false);
        }

        public void ProjectChanged(Project project)
        {
            TargetBuildSelector.Items.Clear();
            if (project.MovieOptions.DefaultBuildTargets != null && project.MovieOptions.DefaultBuildTargets.Length > 0)
            {
                TargetBuildSelector.Items.AddRange(project.MovieOptions.DefaultBuildTargets);
                TargetBuildSelector.Text = project.MovieOptions.DefaultBuildTargets[0];
            }
            else if (project.MovieOptions.TargetBuildTypes != null && project.MovieOptions.TargetBuildTypes.Length > 0)
            {
                TargetBuildSelector.Items.AddRange(project.MovieOptions.TargetBuildTypes);
                string target = project.TargetBuild ?? project.MovieOptions.TargetBuildTypes[0];
                AddTargetBuild(target);
                TargetBuildSelector.Text = target;
            }
            else
            {
                string target = project.TargetBuild ?? "";
                AddTargetBuild(target);
                TargetBuildSelector.Text = target;
            }
            EnableTargetBuildSelector(true);
        }

        internal void AddTargetBuild(string target)
        {
            if (target == null) return;
            target = target.Trim();
            if (target.Length > 0 && !TargetBuildSelector.Items.Contains(target)) 
                TargetBuildSelector.Items.Insert(0, target);
        }

        
        public void ToggleDebugRelease()
        {
            ConfigurationSelector.SelectedIndex = (ConfigurationSelector.SelectedIndex + 1) % 2;
        }
    }

    /// <summary>
    /// The "Project" menu for FD's main menu
    /// </summary>
    public class ProjectMenu : ToolStripMenuItem
    {
        public ToolStripMenuItem NewProject;
        public ToolStripMenuItem OpenProject;
        public ToolStripMenuItem ImportProject;
        public ToolStripMenuItem CloseProject;
        public ToolStripMenuItem OpenResource;
        public ToolStripMenuItem TestMovie;
        public ToolStripMenuItem RunProject;
        public ToolStripMenuItem BuildProject;
        public ToolStripMenuItem CleanProject;
        public ToolStripMenuItem Properties;

        private List<ToolStripItem> AllItems;

        public ProjectMenu()
        {
            AllItems = new List<ToolStripItem>();

            NewProject = new ToolStripMenuItem(TextHelper.GetString("Label.NewProject"), Icons.NewProject.Img);
            OpenProject = new ToolStripMenuItem(TextHelper.GetString("Label.OpenProject"));
            ImportProject = new ToolStripMenuItem(TextHelper.GetString("Label.ImportProject"));
            CloseProject = new ToolStripMenuItem(TextHelper.GetString("Label.CloseProject"));
            OpenResource = new ToolStripMenuItem(TextHelper.GetString("Label.OpenResource"), PluginBase.MainForm.FindImage("209"));
            TestMovie = new ToolStripMenuItem(TextHelper.GetString("Label.TestMovie"), Icons.GreenCheck.Img);
            RunProject = new ToolStripMenuItem(TextHelper.GetString("Label.RunProject"));
            BuildProject = new ToolStripMenuItem(TextHelper.GetString("Label.BuildProject"), Icons.Gear.Img);
            CleanProject = new ToolStripMenuItem(TextHelper.GetString("Label.CleanProject"));
            Properties = new ToolStripMenuItem(TextHelper.GetString("Label.Properties"), Icons.Options.Img);

            //AllItems.Add(NewProject);
            //AllItems.Add(OpenProject);
            //AllItems.Add(ImportProject);
            AllItems.Add(CloseProject);
            AllItems.Add(OpenResource);
            AllItems.Add(TestMovie);
            AllItems.Add(RunProject);
            AllItems.Add(BuildProject);
            AllItems.Add(CleanProject);
            AllItems.Add(Properties);

            PluginBase.MainForm.RegisterShortcut("Project.NewProject", NewProject);
            PluginBase.MainForm.RegisterShortcut("Project.OpenProject", OpenProject);
            PluginBase.MainForm.RegisterShortcut("Project.ImportProject", ImportProject);
            PluginBase.MainForm.RegisterShortcut("Project.CloseProject", CloseProject);
            PluginBase.MainForm.RegisterShortcut("Project.OpenResource", Keys.Control | Keys.R, OpenResource);
            PluginBase.MainForm.RegisterShortcut("Project.TestMovie", Keys.F5, TestMovie);
            PluginBase.MainForm.RegisterShortcut("Project.RunProject", RunProject);
            PluginBase.MainForm.RegisterShortcut("Project.BuildProject", Keys.F8, BuildProject);
            PluginBase.MainForm.RegisterShortcut("Project.CleanProject", Keys.Shift | Keys.F8, CleanProject);
            PluginBase.MainForm.RegisterShortcut("Project.Properties", Properties);

            base.Text = TextHelper.GetString("Label.Project");
            base.DropDownItems.Add(NewProject);
            base.DropDownItems.Add(OpenProject);
            base.DropDownItems.Add(ImportProject);
            base.DropDownItems.Add(CloseProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(OpenResource);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(TestMovie);
            base.DropDownItems.Add(RunProject);
            base.DropDownItems.Add(BuildProject);
            base.DropDownItems.Add(CleanProject);
            base.DropDownItems.Add(new ToolStripSeparator());
            base.DropDownItems.Add(Properties);
        }

        public bool ProjectItemsEnabled
        {
            set
            {
                RunProject.Enabled = value;
                CloseProject.Enabled = value;
                TestMovie.Enabled = value;
                BuildProject.Enabled = value;
                CleanProject.Enabled = value;
                Properties.Enabled = value;
                OpenResource.Enabled = value;
            }
        }

        public bool ProjectItemsEnabledForBuild
        {
            set
            {
                RunProject.Enabled = value;
                CloseProject.Enabled = value;
                TestMovie.Enabled = value;
                BuildProject.Enabled = value;
                CleanProject.Enabled = value;
            }
        }

        public bool AllItemsEnabled
        {
            set
            {
                foreach (ToolStripItem item in DropDownItems)
                {
                    // Toggle items only if it's our creation
                    if (AllItems.Contains(item)) item.Enabled = value;
                }
            }
        }
    }

}
