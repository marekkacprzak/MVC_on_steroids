using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Windsor;
using NHibernate;

namespace MVCPresentation.Web.Features.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false,Inherited = true)]
    public sealed class NoSavesAttribute : Attribute
    {
    }

    public class TransactionFilter :  IAuthorizationFilter, IActionFilter
    {
        private readonly ISession _session;
        private ITransaction _tx;

        public TransactionFilter(ISession session)
        {
            _session = session;
        }

        public void OnAuthorization(AuthorizationContext ctx)
        {
            if (IsApplicable(ctx))
            {
                _tx = _session.BeginTransaction();
            }
        }

        public void OnActionExecuting(ActionExecutingContext ctx)
        {}

        public void OnActionExecuted(ActionExecutedContext ctx)
        {
            if (IsApplicable(ctx))
            {
                ManageTransaction(ctx);
            }
        }

        private void ManageTransaction(ActionExecutedContext ctx)
        {
            if (ModelIsValid(ctx) && IsApplicable(ctx))
            {
                _tx.Commit();
            }
            else
            {
                _tx.Rollback();
            }
        }

        public static bool IsApplicable(ControllerContext ctx, ActionDescriptor actionDescriptor)
        {
            return ctx.IsChildAction == false && actionDescriptor.IsDefined(typeof (NoSavesAttribute), false) == false;
        }

        private static bool IsApplicable(ActionExecutedContext ctx)
        {
            return IsApplicable(ctx, ctx.ActionDescriptor);
        }

        private static bool IsApplicable(AuthorizationContext ctx)
        {
            return IsApplicable(ctx, ctx.ActionDescriptor);
        }

        private static bool ModelIsValid(ControllerContext ctx)
        {
            return ((Controller)ctx.Controller).ModelState.IsValid;
        }
    }

    public class TransactionFilterProvider : IFilterProvider
    {
        private readonly IWindsorContainer _container;

        public TransactionFilterProvider(IWindsorContainer container)
        {
            _container = container;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (TransactionFilter.IsApplicable(controllerContext, actionDescriptor))
                yield return new Filter(new TransactionFilter(_container.Resolve<ISession>()), FilterScope.Global, 0);
        }
    }
}