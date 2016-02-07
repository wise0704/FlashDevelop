using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PluginCore.Managers;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.WinForms.Adapters;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace PluginCore.Controls
{
    /// <summary>
    /// RichTextBox-based tooltip
    /// </summary>
    public class RichToolTip : IEventHandler
    {
        public delegate void UpdateTipHandler(Control sender, Point mousePosition);

        // events
        public event UpdateTipHandler OnUpdateSimpleTip;
        public event CancelEventHandler OnShowing;
        public event EventHandler OnHidden;

        // constants
        protected const int ClientLimitBottom = 26;
        protected const string BaseStyle = @"body {{margin: 0; padding: 0px; font: {0}pt {1}; margin: 0; background-color: {2}}}
pre {{ border: solid 1px gray; background-color:#eee; padding: 1em; white-space: pre-wrap; }}
table {{ border-collapse:collapse; }}
th {{ text-align: left; border: 1px solid #000; background-color: #DDD; padding: 2px 3px 2px 3px; }}
td {{ border: 1px solid #000; padding: 2px 3px 2px 3px; }}";

        // controls
        protected InactiveForm host;
        protected HtmlPanelEx toolTipRTB;
        protected string rawText;
        protected Point mousePos;

        protected ICompletionListHost owner;    // We could just use Control here, or pass a reference on each related call, as Control may be a problem with default implementation

        #region Public Properties

        public bool Focused
        {
            get { return toolTipRTB.Focused; }
        }

        public bool Visible
        {
            get { return host.Visible; }
        }

        public Size Size
        {
            get { return host.Size; }
            set { host.Size = value; }
        }

        public Point Location
        {
            get { return host.Location; }
            set { host.Location = value; }
        }

        public string RawText
        {
            get { return rawText; }
            set
            {
                SetText(value, true);
            }
        }

        public bool Selectable
        {
            get { return toolTipRTB.IsSelectionEnabled; }
            set
            {
                toolTipRTB.IsSelectionEnabled = value;
            }
        }

        public string Text
        {
            get { return toolTipRTB.Text; }
            set
            {
                SetText(value, true);
            }
        }

        #endregion

        #region Control creation

        public RichToolTip(ICompletionListHost owner)
        {
            EventManager.AddEventHandler(this, EventType.ApplyTheme);

            // host
            host = new InactiveForm();
            host.FormBorderStyle = FormBorderStyle.None;
            host.ShowInTaskbar = false;
            host.TopMost = true;
            host.StartPosition = FormStartPosition.Manual;
            // Use the form backcolor so we can use any color as the border
            host.BackColor = Color.Black;
            host.KeyPreview = true;
            host.KeyDown += Host_KeyDown;

            this.owner = owner;

            // html panel
            toolTipRTB = new HtmlPanelEx();
            toolTipRTB.BackColor = SystemColors.Info;
            toolTipRTB.ForeColor = SystemColors.InfoText;
            toolTipRTB.Location = new Point(1, 1);
            toolTipRTB.Padding = new Padding(2);
            toolTipRTB.Size = new Size(host.Width - 2, host.Height - 2);
            toolTipRTB.BaseStylesheet = string.Format(BaseStyle, 
    PluginBase.MainForm.Settings.DefaultFont.SizeInPoints.ToString(CultureInfo.InvariantCulture),
    PluginBase.MainForm.Settings.DefaultFont.Name, GetColorString(SystemColors.Info));
            toolTipRTB.Text = "";
            toolTipRTB.LostFocus += Host_LostFocus;
            host.Controls.Add(toolTipRTB);
        }
        
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                IMainForm mainForm = PluginBase.MainForm;
                Color fore = mainForm.GetThemeColor("RichToolTip.ForeColor");
                Color back = mainForm.GetThemeColor("RichToolTip.BackColor");
                toolTipRTB.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
                toolTipRTB.BaseStylesheet = string.Format(BaseStyle,
                    mainForm.Settings.DefaultFont.SizeInPoints.ToString(CultureInfo.InvariantCulture),
                    mainForm.Settings.DefaultFont.Name, GetColorString(back == Color.Empty ? SystemColors.Info : back));
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void Host_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                Hide();
        }

        protected virtual void Host_LostFocus(object sender, EventArgs e)
        {
            if (!owner.Owner.ContainsFocus)
                Hide();
        }

        #endregion

        #region Tip Methods

        public bool AutoSize()
        {
            return AutoSize(0);
        }
        public bool AutoSize(int availableWidth)
        {
            return AutoSize(availableWidth, 1024);
        }
        public bool AutoSize(int availableWidth, int maxWidth)
        {
            bool tooSmall = false;

            // tooltip larger than the window: wrap
            var screenArea = Screen.FromControl(owner.Owner).WorkingArea;
            int limitLeft = screenArea.Left + 1;
            int limitRight = screenArea.Right - 1;
            int limitBottom = screenArea.Bottom - ClientLimitBottom;
            //
            int maxW = availableWidth > 0 ? availableWidth : limitRight - limitLeft;
            if (maxW > maxWidth && maxWidth > 0)
                maxW = maxWidth;

            Size txtSize = toolTipRTB.GetPreferredSize(Size.Empty);
            // For some reason we have to take into account the padding set in body, it shouldn't be that way
            int w = txtSize.Width;
            int h = txtSize.Height;
            if (w > maxW)
            {
                w = maxW;
                if (w < 200)
                {
                    w = 200;
                    tooSmall = true;
                }

                txtSize = toolTipRTB.GetPreferredSize(new Size(w - 2, 0));
                w = txtSize.Width;
                h = txtSize.Height;
            }

            if (h > limitBottom - host.Top)
            {
                w += SystemInformation.VerticalScrollBarWidth;
                h = limitBottom - host.Top - 2;
            }
            toolTipRTB.Size = new Size(w, h);
            host.Size = new Size(toolTipRTB.Size.Width + 2, toolTipRTB.Size.Height + 2);

            if (host.Left < limitLeft)
                host.Left = limitLeft;
            
            if (host.Left + host.Width > limitRight)
                host.Left = limitRight - host.Width;

            return !tooSmall;
        }

        public void ShowAtMouseLocation(string text)
        {
            if (string.CompareOrdinal("<body><div style=\"margin:0\">" + text + "</div></body>", Text) != 0)
            {
                host.Visible = false;
                Text = text;
            }
            ShowAtMouseLocation();
        }

        public void ShowAtMouseLocation()
        {
            mousePos = Control.MousePosition;
            host.Left = mousePos.X;
            var screenArea = Screen.FromPoint(mousePos).WorkingArea;
            if (host.Right > screenArea.Right)
            {
                host.Left -= (host.Right - screenArea.Right);
            }
            host.Top = mousePos.Y - host.Height - 10;

            if (host.Top < 5)
            {
                // Let's be sure we don't go offscreen
                int downSpace = screenArea.Bottom - ClientLimitBottom - mousePos.Y - 10;
                int topSpace = mousePos.Y - 15;

                Size tipSize = toolTipRTB.Size;
                if (downSpace > topSpace)
                {
                    host.Top = mousePos.Y + 10;

                    if (host.Height > downSpace)
                    {
                        tipSize.Height = downSpace - 2;
                        if (toolTipRTB.Height >= toolTipRTB.ActualSize.Height &&
                            tipSize.Height < toolTipRTB.ActualSize.Height)
                        {
                            tipSize.Width += SystemInformation.VerticalScrollBarWidth;
                        }
                        toolTipRTB.Size = tipSize;
                        host.Size = new Size(toolTipRTB.Width + 2, toolTipRTB.Height + 2);
                    }
                }
                else
                {
                    host.Top = 5;

                    tipSize.Height = topSpace - 2;
                    if (toolTipRTB.Height >= toolTipRTB.ActualSize.Height &&
                        tipSize.Height < toolTipRTB.ActualSize.Height)
                    {
                        tipSize.Width += SystemInformation.VerticalScrollBarWidth;
                    }
                    toolTipRTB.Size = tipSize;
                    host.Size = new Size(toolTipRTB.Width + 2, toolTipRTB.Height + 2);
                }
            }
            Show();
        }

        public virtual void UpdateTip()
        {
            if (OnUpdateSimpleTip != null) OnUpdateSimpleTip(owner.Owner, mousePos);
        }

        public virtual void Hide()
        {
            if (host.Visible)
            {
                host.Visible = false;
                toolTipRTB.ResetText();
                if (OnHidden != null) OnHidden(this, EventArgs.Empty);
            }
        }

        public virtual void Show()
        {
            if (!host.Visible)
            {
                if (OnShowing != null)
                {
                    var cancelArgs = new CancelEventArgs();
                    OnShowing(this, cancelArgs);
                    if (cancelArgs.Cancel)
                    {
                        Hide();
                        return;
                    }
                }

                // Not really needed to set an owner, it has some advantages currently unused
                host.Owner = null;  // To avoid circular references that may happen because of Floating -> Docking panels
                host.Show(owner.Owner);
            }
        }

        public void SetText(string rawText, bool redraw)
        {
            this.rawText = rawText ?? "";
            if (redraw)
                Redraw();
        }

        public void Redraw()
        {
            Redraw(true);
        }
        public void Redraw(bool autoSize)
        {
            toolTipRTB.Text = "<body><div style=\"margin:0\">" + rawText + "</div></body>";

            /*IMainForm mainForm = PluginBase.MainForm;
            Color fore = mainForm.GetThemeColor("RichToolTip.ForeColor");
            Color back = mainForm.GetThemeColor("RichToolTip.BackColor");
            toolTip.BackColor = back == Color.Empty ? SystemColors.Info : back;
            toolTip.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            toolTipRTB.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            toolTipRTB.BaseStylesheet = string.Format(BaseStyle,
                mainForm.Settings.DefaultFont.SizeInPoints.ToString(CultureInfo.InvariantCulture),
                mainForm.Settings.DefaultFont.Name, GetColorString(back == Color.Empty ? SystemColors.Info : back));*/

            if (autoSize)
                AutoSize();
        }

        public bool IsMouseInside()
        {
            return host.Bounds.Contains(Control.MousePosition);
        }

        private static string GetColorString(Color color)
        {
            return string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, color.A);
        }

        #endregion

        protected class HtmlPanelEx : HtmlPanel
        {
            public SizeF ActualSize
            {
                get { return base._htmlContainer.ActualSize; }
            }

            public override Size GetPreferredSize(Size proposedSize)
            {
                Graphics g = Utils.CreateGraphics(this);
                if (g != null)
                {
                    using (g)
                    using (var ig = new GraphicsAdapter(g, UseGdiPlusTextRendering))
                    {
                        var newSize = HtmlRendererUtils.Layout(ig, _htmlContainer.HtmlContainerInt,
                            new RSize(proposedSize.Width - Padding.Horizontal, proposedSize.Height - Padding.Vertical),
                            new RSize(MinimumSize.Width - Padding.Horizontal, MinimumSize.Height - Padding.Vertical),
                            new RSize(MaximumSize.Width - Padding.Horizontal, MaximumSize.Height - Padding.Vertical),
                            proposedSize.Width < 1, proposedSize.Height < 1);

                        return new Size((int)Math.Ceiling(newSize.Width + Padding.Horizontal),
                                (int)Math.Ceiling(newSize.Height + Padding.Vertical));
                    }
                }

                return Size.Empty;
            }
        }
    }

}
