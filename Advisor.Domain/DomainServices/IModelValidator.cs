namespace Advisor.Domain.DomainServices;
public interface IModelValidator<T>
{
    void Validate(T model);
}
