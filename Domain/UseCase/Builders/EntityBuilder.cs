using System;

namespace Domain.UseCase.Builders
{
    public class EntityBuilder
    {
        public static T Call<T>(object obj)
        {
            var entity = Activator.CreateInstance<T>();
            foreach (var field in obj.GetType().GetProperties())
            {
                var value = obj.GetType().GetProperty(field.Name).GetValue(obj);
                if(value != null)
                {
                    var prop = entity.GetType().GetProperty(field.Name);
                    if(prop != null) prop.SetValue(entity, value);
                }
            }

            return entity;
        }
    }
}
