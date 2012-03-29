using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace meituan.Helper
{
    public class PageMessage
    {
        public string cityid { get; set; }
        public meituan.Model.City city { get; set; }
        public meituan.Model.Deal _deal { get; set; }
    }
    public static class MessengerExtension
    {
        public static void Send<T>(this IMessenger messenger, T body, object token)
        {
            Messenger.Default.Send<GenericMessage<T>>(new GenericMessage<T>(body), token);
        }

        public static void Register<T>(this Messenger messenger, object recipient, object token, Action<T> action)
        {
            Messenger.Default.Register<GenericMessage<T>>(recipient, token, msg =>
            {
                action(msg.Content);
            });
        }
    }
}
