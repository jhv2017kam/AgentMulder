﻿using System.Linq;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.StructureMap.Registrations
{
    public abstract class StructureMapConventionBase : FilteredRegistrationBase
    {
        protected StructureMapConventionBase(ITreeNode registrationRootElement)
            : base(registrationRootElement)
        {
            AddFilter(element => element.Constructors.All(
                constructor => constructor.Parameters.All(IsAutoFillable)));
        }

        private bool IsAutoFillable(IParameter parameter)
        {
            return IsChild(parameter.Type) || IsChildArray(parameter.Type);
        }

        private bool IsChildArray(IType type)
        {
            return type.IsArray() && !IsSimple(type.GetScalarType());
        }

        private bool IsChild(IType type)
        {
            return IsPrimitiveArray(type) || (!type.IsArray() && !IsSimple(type));
        }

        private bool IsSimple(IType type)
        {
            // todo fix for enum
            bool isEnum = false;
            IDeclaredType declaredType = type.GetScalarType();
            if (declaredType != null)
            {
                isEnum = declaredType.GetTypeElement() is IEnum;
            }

            return type.IsSimplePredefined() || isEnum;
        }

        private bool IsPrimitiveArray(IType type)
        {
            return type.IsArray() && IsSimple(type.GetScalarType());
        }
    }
}