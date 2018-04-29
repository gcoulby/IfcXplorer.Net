using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xbim.Common;
using Xbim.Common.Federation;
using Xbim.Ifc.Extensions;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace IfcXplorer.ViewModels
{
    /// <summary>
    /// Model view for display top level Xbim Model contents and referenced models
    /// </summary>
    public class XbimModelViewModel : CustomXbimViewModel
    {
        private readonly IIfcProject _project;
        private ObservableCollection<IXbimViewModel> _children;

        public override int EntityLabel => _project.EntityLabel;
        public override IPersistEntity Entity => _project;
        public override string Name => "Project";

        public XbimModelViewModel(IIfcProject project, IXbimViewModel parent)
        {
            Model = project.Model;
            _project = project;
            CreatingParent = parent;
        }
        
        public sealed override IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = new ObservableCollection<IXbimViewModel>();
                foreach (var item in _project.GetSpatialStructuralElements())
                {
                    _children.Add(new SpatialViewModel(item, this));
                }

                if (!(Model is IFederatedModel federation)) return _children;
                    
                foreach (var refModel in federation.ReferencedModels)
                {
                    _children.Add(new XbimRefModelViewModel(refModel, this));
                }
                return _children;
            }
        }
     
        public void AddRefModel(XbimRefModelViewModel xbimModelViewModel)
        {
            _children.Add(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }

        public void RemoveRefModel(XbimRefModelViewModel xbimModelViewModel)
        {
            _children.Remove(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }
    }
}
