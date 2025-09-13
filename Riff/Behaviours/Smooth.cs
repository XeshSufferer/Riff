using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Riff.Behaviors
{
    public class SmoothHoverBehavior : Behavior<View>
    {
        private View _element;
        private bool _isHovered;

        public static readonly BindableProperty HoverScaleProperty =
            BindableProperty.Create(nameof(HoverScale), typeof(double), typeof(SmoothHoverBehavior), 0.9);

        public static readonly BindableProperty AnimationDurationProperty =
            BindableProperty.Create(nameof(AnimationDuration), typeof(uint), typeof(SmoothHoverBehavior), 250u);

        public double HoverScale
        {
            get => (double)GetValue(HoverScaleProperty);
            set => SetValue(HoverScaleProperty, value);
        }

        public uint AnimationDuration
        {
            get => (uint)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            _element = bindable;
            SubscribeToEvents();
        }

        protected override void OnDetachingFrom(View bindable)
        {
            UnsubscribeFromEvents();
            base.OnDetachingFrom(bindable);
            _element = null;
        }

        private void SubscribeToEvents()
        {
#if WINDOWS
            if (_element.Handler != null)
            {
                AttachWindowsEvents();
            }
            else
            {
                _element.HandlerChanged += OnHandlerChanged;
            }
#endif

#if ANDROID || IOS
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTapped;
            _element.GestureRecognizers.Add(tapGesture);
#endif
        }

        private void UnsubscribeFromEvents()
        {
#if WINDOWS
            if (_element != null)
            {
                _element.HandlerChanged -= OnHandlerChanged;
                DetachWindowsEvents();
            }
#endif

#if ANDROID || IOS
            if (_element != null && _element.GestureRecognizers.Count > 0)
            {
                _element.GestureRecognizers.Clear();
            }
#endif
        }

#if WINDOWS
        private void OnHandlerChanged(object sender, EventArgs e)
        {
            if (_element.Handler != null)
            {
                AttachWindowsEvents();
                _element.HandlerChanged -= OnHandlerChanged;
            }
        }

        private void AttachWindowsEvents()
        {
            var nativeView = _element.Handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;
            if (nativeView != null)
            {
                nativeView.PointerEntered += OnWindowsPointerEntered;
                nativeView.PointerExited += OnWindowsPointerExited;
            }
        }

        private void DetachWindowsEvents()
        {
            var nativeView = _element.Handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;
            if (nativeView != null)
            {
                nativeView.PointerEntered -= OnWindowsPointerEntered;
                nativeView.PointerExited -= OnWindowsPointerExited;
            }
        }

        private async void OnWindowsPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isHovered || _element == null) return;
            _isHovered = true;
            
            await _element.ScaleTo(HoverScale, AnimationDuration, Easing.CubicOut);
        }

        private async void OnWindowsPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!_isHovered || _element == null) return;
            _isHovered = false;
            
            await _element.ScaleTo(1.0, AnimationDuration, Easing.CubicOut);
        }
#endif

        private async void OnTapped(object sender, EventArgs e)
        {
            if (_element == null) return;

            if (_isHovered)
            {
                await _element.ScaleTo(1.0, AnimationDuration, Easing.CubicOut);
            }
            else
            {
                await _element.ScaleTo(HoverScale, AnimationDuration, Easing.CubicOut);
            }
            _isHovered = !_isHovered;
        }
    }
}