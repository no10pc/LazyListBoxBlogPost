using System;
using Microsoft.Phone.Controls;
using System.Windows.Media.Animation;
using System.Windows;


namespace meituan
{
    public class MyTransition : TransitionElement
    {
        public const string StoryboardPropertyName = "Storyboard";

        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register(StoryboardPropertyName, typeof(Storyboard), typeof(MyTransition), 
            new PropertyMetadata(null));

        public override ITransition GetTransition(System.Windows.UIElement element)
        {
            return new Transition(element, this.Storyboard);
        }
    }
}
