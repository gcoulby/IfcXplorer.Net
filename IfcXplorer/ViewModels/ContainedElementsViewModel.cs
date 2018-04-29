using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace IfcXplorer.ViewModels
{
    public class ContainedElementsViewModel : CustomXbimViewModel
    {
        private readonly Type _type;
        private readonly IIfcSpatialStructureElement _spatialContainer;
        private List<IXbimViewModel> _children;

        public override int EntityLabel => _spatialContainer.EntityLabel;
        public override IPersistEntity Entity => _spatialContainer;
        public override string Name => _type.Name;

        public ContainedElementsViewModel(IIfcSpatialStructureElement spatialElem, Type type, IXbimViewModel parent)
        {
            _spatialContainer = spatialElem;
            _type = type;
            Model = spatialElem.Model;
            CreatingParent = parent;
        }
        
        public override IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<IXbimViewModel>();
                    foreach (var rel in _spatialContainer.ContainsElements)
                    {
                        foreach (var prod in rel.RelatedElements.Where(e => e.GetType() == _type))
                            _children.Add(new IfcProductModelView(prod, this));
                    }
                }
                return _children;
            }
        }
    }
}
