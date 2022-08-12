﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NpfRatexTest3.ViewModels
{
    public abstract class ViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            //var handlers = PropertyChanged;
            //if(handlers is null)return;

            //var invokation_list = handlers.GetInvocationList();

            //var arg = new PropertyChangedEventArgs(PropertyName);
            //foreach (var action in invokation_list)
            //{
            //    if (action.Target is DispatcherObject disp_object)
            //        disp_object.Dispatcher.Invoke(action, this, arg);
            //    else
            //        action.DynamicInvoke(this, arg);
            //}
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
