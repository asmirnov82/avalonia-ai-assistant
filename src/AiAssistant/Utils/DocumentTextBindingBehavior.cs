using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace AiAssistant.Utils
{
    public class DocumentTextBindingBehavior : Behavior<TextEditor>, IObserver<string>
    {
        private TextEditor? _textEditor;

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<DocumentTextBindingBehavior, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        { }
        
        public void OnNext(string value)
        {
            TextPropertyChanged(value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                _textEditor = AssociatedObject;
                _textEditor.TextChanged += TextChanged;
                this.GetObservable(TextProperty).Subscribe(this);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_textEditor != null)
            {
                _textEditor.TextChanged -= TextChanged;
            }
        }

        private void TextChanged(object sender, EventArgs eventArgs)
        {
            if (_textEditor != null && _textEditor.Document != null)
            {
                Text = _textEditor.Document.Text;
            }
        }

        private void TextPropertyChanged(string text)
        {
            if (_textEditor != null && _textEditor.Document != null && text != null)
            {
                if (text == "")
                {
                    _textEditor.Document.Text = text;
                    _textEditor.CaretOffset = 0;
                }
                else
                {
                    var caretOffset = _textEditor.CaretOffset;
                    _textEditor.Document.Text = text;
                    _textEditor.CaretOffset = caretOffset;
                }
            }
        }
    }
}
