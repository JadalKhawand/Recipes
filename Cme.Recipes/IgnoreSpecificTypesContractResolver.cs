using Newtonsoft.Json.Serialization;
using System;

public class IgnoreSpecificTypesContractResolver : DefaultContractResolver
{
    private readonly Type[] _typesToIgnore;

    public IgnoreSpecificTypesContractResolver(params Type[] typesToIgnore)
    {
        _typesToIgnore = typesToIgnore ?? throw new ArgumentNullException(nameof(typesToIgnore));
    }

    protected override JsonContract CreateContract(Type objectType)
    {
        if (_typesToIgnore.Any(t => t == objectType))
        {
            return null; 
        }

        return base.CreateContract(objectType);
    }
}
