using System.Linq;

namespace SpectraWay.Extension
{
    public static class Map
    {
        public static T InitFieldsFrom<T, TU>(this T target, TU source) where T  : class
                                                                    where TU : class
        {
            // get property list of the target object.
            // this is a reflection extension which simply gets properties (CanWrite = true).
            var tprops = target.GetType().GetProperties().Select(x=>x.Name);

            tprops.ToList().ForEach(prop =>
            {
                // check whether source object has the the property
                var sp = source.GetType().GetProperty(prop);
                if (sp != null && sp.GetIndexParameters().Length == 0)
                {
                    // if yes, copy the value to the matching property
                    var value = sp.GetValue(source, null);
                    var propertyInfo = target.GetType().GetProperty(prop);
                    if (propertyInfo?.CanWrite ?? false)
                    {
                        propertyInfo?.SetValue(target, value, null);
                    }
                }
            });

            return target;
        }
    }
}