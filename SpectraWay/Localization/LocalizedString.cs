using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SpectraWay.Localization
{
    public interface IListenLocalizedString
    {
        void OnLocalizedStringChanged(string key);
    }

    public class LocalizedString : INotifyPropertyChanged
    {
        private string key;
        private string _default;
        public string Default
        {
            get { return _default; }
            private set
            {
                _default = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("ValueUpperCase");
                OnPropertyChanged("ValueLowerCase");
            }
        }
        private string _localized;
        public string Localized
        {
            get { return _localized; }
            set
            {
                _localized = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("ValueUpperCase");
                OnPropertyChanged("ValueLowerCase");
                SendChanges();
            }
        }

        public LocalizedString(string key, string defaultValue = "")
        {
            this.key = key;
            Default = defaultValue;
            Localized = null;
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

        public string Value
        {
            get
            {
                return Localized ?? Default;
            }
        }

        public string ValueUpperCase
        {
            get
            {
                string result = Localized ?? Default;
                return result.ToUpper(CultureInfo.CurrentCulture);
            }
        }

        public string ValueLowerCase
        {
            get
            {
                string result = Localized ?? Default;
                return result.ToLower(CultureInfo.CurrentCulture);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region WeakReferencePart
        private void SendChanges()
        {
            WeakReference[] weakArray;
            lock (weakReferences)
            {
                weakArray = weakReferences.ToArray();
                foreach (var weakLink in weakArray)
                {
                    var target = weakLink.Target as IListenLocalizedString;
                    if (target != null)
                    {
                        target.OnLocalizedStringChanged(key);
                    }
                    else
                    {
                        weakReferences.Remove(weakLink);
                    }
                }
            }
        }

        private List<WeakReference> weakReferences = new List<WeakReference>();
        public void AddWeakHandler(IListenLocalizedString listenObj)
        {
            lock (weakReferences)
            {
                CleanRefference();
                foreach (var weakReference in weakReferences)
                {
                    if (weakReference.IsAlive && weakReference.Target == listenObj)
                    {
                        return;
                    }
                }
                weakReferences.Add(new WeakReference(listenObj));
            }
        }

        private void CleanRefference()
        {
            lock (weakReferences)
            {
                var weakArray = weakReferences.ToArray();
                foreach (var weakLink in weakArray)
                {
                    if (!weakLink.IsAlive)
                    {
                        weakReferences.Remove(weakLink);
                    }
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return Value;
        }
    }
    
}
