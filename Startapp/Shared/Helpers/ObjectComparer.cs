using System.Collections;

namespace Startapp.Shared.Helpers
{
    public class ObjectComparer
    {
        public static bool Equals(object left, object right)
        {
            //Compare the references
            if (object.ReferenceEquals(right, null))
                return false;
            if (object.ReferenceEquals(left, right))
                return true;

            //Compare the types
            if (left.GetType() != right.GetType())
                return false;

            //Get all property infos of the right object
            var propertyInfos = right.GetType().GetProperties();

            //Compare the property values of the left and right object
            foreach (var propertyInfo in propertyInfos)
            {
                var othersValue = propertyInfo.GetValue(right);
                var currentsValue = propertyInfo.GetValue(left);
                if (othersValue == null && currentsValue == null)
                    continue;

                //Comparison if the property is a generic (IList type)
                if ((currentsValue is IList && propertyInfo.PropertyType.IsGenericType) ||
                    (othersValue is IList && propertyInfo.PropertyType.IsGenericType))
                {
                    //here we work with dynamics because don't need to care about the generic type
                    dynamic cur = currentsValue;
                    dynamic oth = othersValue;
                    if (cur != null && cur.Count > 0)
                    {
                        var result = false;
                        foreach (object cVal in cur)
                        {
                            foreach (object oVal in oth)
                            {
                                //Recursively call the Equal method
                                var areEqual = Equals(cVal, oVal);
                                if (!areEqual) continue;

                                result = true;
                                break;
                            }

                        }
                        if (result == false)
                            return false;
                    }
                }
                else
                {
                    //Comparison for properties of a non collection type
                    var curType = currentsValue.GetType();

                    //Comparison for primitive types
                    if (curType.IsValueType || currentsValue is string)
                    {
                        var areEquals = currentsValue.Equals(othersValue);
                        if (areEquals == false)
                            return false;      //This is the out point for this methods
                    }
                    else
                    {
                        //values are complex/classes
                        //that's why we have to recursively call the Equals methods
                        var areEqual = Equals(currentsValue, othersValue);
                        if (areEqual == false)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
