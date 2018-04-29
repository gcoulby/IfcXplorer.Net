using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace IfcXplorer.ViewModels
{
    public class IfcProductModelView : CustomXbimViewModel
    {
        private readonly IIfcProduct _product;
        private List<IXbimViewModel> _children;

        public override int EntityLabel => _product.EntityLabel;
        public override IPersistEntity Entity => _product;

        public override string Name => $"{_product.ExpressType.ExpressName.Substring(3)} #{_product.EntityLabel}";

        public IfcProductModelView(IIfcProduct prod, IXbimViewModel parent)
        {
            CreatingParent = parent;
            _product = prod;
        }

        public override IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = new List<IXbimViewModel>();
                var breakdown = _product.IsDecomposedBy.ToList();
                if (!breakdown.Any()) return _children;

                foreach (var rel in breakdown)
                {
                    foreach (var prod in rel.RelatedObjects.OfType<IIfcProduct>())
                    {
                        _children.Add(new IfcProductModelView(prod, this));
                    }
                }
                return _children;
            }
        }
    }
}
