using System.Text.RegularExpressions;
namespace Armageddon.Mobile.Behaviors
{
    public class NumericOnlyBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            // Allow only digits (0-9)
            var regex = new Regex("^[0-9]*\\.?[0-9]*$");

            if (!regex.IsMatch(e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }
}