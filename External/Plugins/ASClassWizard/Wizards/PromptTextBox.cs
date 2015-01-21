// There is a native API to show a prompt in TextBoxes and ComboBoxes, but I'm not using it in order to not have native code...

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASClassWizard.Wizards
{
    public class PromptTextBox : TextBox
    {

        private string _prompt = "";
        [DefaultValue("")]
        [Localizable(true)]
        public string Prompt
        {
            get { return _prompt; }
            set
            {
                if (value == null) value = string.Empty;

                _prompt = value;
                if (string.IsNullOrEmpty(_text) && _prompt != string.Empty)
                {
                    base.Text = value;
                }
            }
        }

        private string _text = "";
        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == null) value = string.Empty;

                _text = value;
                if (_text == string.Empty && !string.IsNullOrEmpty(Prompt))
                {
                    base.Text = Prompt;
                }
                else
                {
                    base.Text = value;
                }
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (_text == string.Empty) base.Text = string.Empty;
        }

        protected override void OnLeave(EventArgs e)
        {
            if (base.Text == string.Empty && !string.IsNullOrEmpty(Prompt)) base.Text = Prompt;
            base.OnLeave(e);
        }
    }
}
