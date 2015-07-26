using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MP3_SQL_Lib.MVVMMessages
{
    public class SelectFolderMessage
    {
        public Action<string> CallBack { get; set; }
        public SelectFolderMessage(Action<string> callback)
        {
            CallBack = callback;
        }
    }

    public class ShowSaveMessage : NotificationMessageAction<MessageBoxResult>
    {
        /// <summary>
        /// Messagebox.show with result for MVVM Light Messaging
        /// </summary>
        public ShowSaveMessage(object Sender, Action<MessageBoxResult> callback)
            : base(Sender, "GetPassword", callback)
        {

        }
    }
}
