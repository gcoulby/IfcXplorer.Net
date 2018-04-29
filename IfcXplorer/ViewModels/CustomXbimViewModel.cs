using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc.ViewModels;

namespace IfcXplorer.ViewModels
{
    public abstract class CustomXbimViewModel : IXbimViewModel
    {
        public abstract IEnumerable<IXbimViewModel> Children { get; }
        public abstract string Name { get; }
        public abstract int EntityLabel { get; }

        private bool _isSelected;
        private bool _isExpanded;

        public bool HasItems => Children.Any();

        public abstract IPersistEntity Entity { get; }

        public IXbimViewModel CreatingParent { get; set; }
        public IModel Model { get; set; }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }

        [field: NonSerialized] //don't serialize events
        protected event PropertyChangedEventHandler PropertyChanged;


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
