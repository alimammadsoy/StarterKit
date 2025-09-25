using StarterKit.Application.Exceptions;
using System.Linq.Expressions;

namespace StarterKit.Application.Consts
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySortingAndFiltering<T>(this IQueryable<T> query, string columnName, string orderBy, string filterValue, List<string> ignoredProperties = null)
        {
            if (!string.IsNullOrWhiteSpace(filterValue))
            {
                query = query.ApplyFilter(filterValue, ignoredProperties);
            }

            if (!string.IsNullOrWhiteSpace(columnName))
            {
                query = query.ApplySorting(columnName, orderBy);
            }

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string columnName, string orderBy)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return query;

            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = "asc";


            var parameter = Expression.Parameter(typeof(T), "x");
            Expression propertyAccess = parameter;

            try
            {
                foreach (var property in columnName.Split('.'))
                {
                    propertyAccess = Expression.PropertyOrField(propertyAccess, property);
                }
            }
            catch (ArgumentException)
            {
                throw new ExtensionException($"Column name '{columnName}' is not valid for sorting.");
            }

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var methodName = orderBy.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";


            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, propertyAccess.Type },
                query.Expression,
                Expression.Quote(orderByExpression)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string filterValue, List<string> ignoredProperties = null)
        {
            if (string.IsNullOrWhiteSpace(filterValue))
                return query;

            ignoredProperties ??= new List<string>();

            var parameter = Expression.Parameter(typeof(T), "x");
            var filterExpressions = new List<Expression>();

            foreach (var property in typeof(T).GetProperties())
            {
                if (!property.CanRead)
                    continue;

                var propertyName = property.Name;
                if (ignoredProperties.Contains(propertyName))
                    continue;

                if (property.PropertyType == typeof(string))
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(propertyAccess, toLowerMethod);
                    var constant = Expression.Constant(filterValue.ToLower());
                    var containsExpression = Expression.Call(propertyToLower, "Contains", null, constant);
                    filterExpressions.Add(containsExpression);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var nestedPropertyAccess = Expression.Property(parameter, property);
                    foreach (var nestedProperty in property.PropertyType.GetProperties())
                    {
                        if (!nestedProperty.CanRead)
                            continue;

                        var nestedFullName = $"{property.Name}.{nestedProperty.Name}";
                        if (ignoredProperties.Contains(nestedFullName))
                            continue;

                        if (nestedProperty.PropertyType == typeof(string))
                        {
                            var nestedPropertyAccessExp = Expression.Property(nestedPropertyAccess, nestedProperty);
                            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                            var nestedPropertyToLower = Expression.Call(nestedPropertyAccessExp, toLowerMethod);
                            var constant = Expression.Constant(filterValue.ToLower());
                            var containsExpression = Expression.Call(nestedPropertyToLower, "Contains", null, constant);
                            filterExpressions.Add(containsExpression);
                        }
                    }
                }
            }

            if (filterExpressions.Count == 0)
                return query;

            var finalFilterExpression = filterExpressions.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda<Func<T, bool>>(finalFilterExpression, parameter);
            return query.Where(lambda);
        }

        public static IQueryable<T> FilterByExamDate<T>(this IQueryable<T> query, string examDatePropertyName, DateTime? filterDate)
        {
            if (!filterDate.HasValue || string.IsNullOrWhiteSpace(examDatePropertyName))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression propertyAccess = parameter;
            foreach (var part in examDatePropertyName.Split('.'))
            {
                propertyAccess = Expression.PropertyOrField(propertyAccess, part);
            }

            // Set to UTC with DateTimeKind.Utc to avoid PostgreSQL exception
            var startOfDay = DateTime.SpecifyKind(filterDate.Value.Date, DateTimeKind.Utc);
            var endOfDay = DateTime.SpecifyKind(filterDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            // Make constants match the type (DateTime or Nullable<DateTime>)
            Expression startConstant = Expression.Constant(startOfDay, propertyAccess.Type);
            Expression endConstant = Expression.Constant(endOfDay, propertyAccess.Type);

            var greaterThanOrEqual = Expression.GreaterThanOrEqual(propertyAccess, startConstant);
            var lessThanOrEqual = Expression.LessThanOrEqual(propertyAccess, endConstant);

            var filterExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);

            return query.Where(lambda);
        }


    }
}
