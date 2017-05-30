using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    /// <summary>
    /// A manager class for navigation through document tabs.
    /// </summary>
    internal class TabbingManager : IEventHandler, IDisposable
    {
        private static TabbingManager instance;
        private Container components;
        private Timer timer;
        private List<ITabbedDocument> history;
        private int sequentialIndex;

        private TabbingManager()
        {
            components = new Container();
            timer = new Timer(components);
            timer.Tick += Timer_Tick;
            history = new List<ITabbedDocument>();
            sequentialIndex = 0;
        }

        ~TabbingManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
        }

        internal static void Initialize()
        {
            instance = new TabbingManager();

            EventManager.AddEventHandler(instance, EventType.ShortcutKey, HandlingPriority.Low);
            Globals.MainForm.RegisterShortcut("View.TabNext", Keys.Control | Keys.Tab);
            Globals.MainForm.RegisterShortcut("View.TabPrevious", Keys.Control | Keys.Shift | Keys.Tab);
            Globals.MainForm.RegisterShortcut("View.TabNextSequential", Keys.Control | Keys.PageDown);
            Globals.MainForm.RegisterShortcut("View.TabPreviousSequential", Keys.Control | Keys.PageUp);
        }

        /// <summary>
        /// Adds a document to the tab history.
        /// </summary>
        internal static void AddTabHistory(ITabbedDocument document)
        {
            if (!instance.timer.Enabled)
            {
                instance.history.Remove(document);
                instance.history.Add(document);
            }
        }

        /// <summary>
        /// Removes a document from the tab history.
        /// </summary>
        internal static void RemoveTabHistory(ITabbedDocument document)
        {
            instance.history.Remove(document);
            if (Globals.Settings.SequentialTabbing)
            {
                if (instance.sequentialIndex == 0) NavigateTabsSequentially(0);
                else NavigateTabsSequentially(-1);
            }
            else NavigateTabHistory(0);
        }

        /// <summary>
        /// Sets the current index to the index of the specified document.
        /// </summary>
        internal static void UpdateSequentialIndex(ITabbedDocument document)
        {
            int index = Array.IndexOf(Globals.MainForm.Documents, document);
            if (index >= 0)
            {
                instance.sequentialIndex = index;
            }
        }

        /// <summary>
        /// Navigates through the documents in tabs sequentially.
        /// </summary>
        internal static void NavigateTabsSequentially(int direction)
        {
            Navigate(direction, Globals.MainForm.Documents);
        }

        /// <summary>
        /// Performs Visual Studio style keyboard tab navigation: similar to Alt-Tab.
        /// </summary>
        internal static void NavigateTabHistory(int direction)
        {
            Navigate(direction, instance.history);
        }

        /// <summary>
        /// Navigates through the list using the specified direction.
        /// </summary>
        private static void Navigate(int direction, IList<ITabbedDocument> list)
        {
            if (direction == 0)
            {
                if (list.Count > 0)
                {
                    list[0].Activate();
                }
                return;
            }
            int c = list.Count - 1;
            if (c > 0)
            {
                int i = list.IndexOf(Globals.CurrentDocument);
                if (i >= 0)
                {
                    if (direction > 0)
                    {
                        if (i < c) list[i + 1].Activate();
                        else list[0].Activate();
                    }
                    else
                    {
                        if (i > 0) list[i - 1].Activate();
                        else list[c].Activate();
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if the Control key has been released
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                timer.Stop();
                AddTabHistory(Globals.MainForm.CurrentDocument);
            }
        }

        void IEventHandler.HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ShortcutKey)
            {
                string command = ((ShortcutKeyEvent) e).Command;
                switch (command)
                {
                    case "View.TabNext":
                    case "View.TabNextSequential":
                        timer.Start();
                        if (command == "View.TabNextSequential" || Globals.Settings.SequentialTabbing)
                        {
                            NavigateTabsSequentially(1);
                        }
                        else
                        {
                            NavigateTabHistory(1);
                        }
                        e.Handled = true;
                        break;

                    case "View.TabPrevious":
                    case "View.TabPreviousSequential":
                        timer.Start();
                        if (command == "View.TabPreviousSequential" || Globals.Settings.SequentialTabbing)
                        {
                            NavigateTabsSequentially(-1);
                        }
                        else
                        {
                            NavigateTabHistory(-1);
                        }
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}
