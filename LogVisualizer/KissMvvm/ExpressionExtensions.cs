﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogVisualizer.KissMvvm
{
    public static class ExpressionExtensions
    {
        public static String GetMemberName<T>(this Expression<Func<T>> source)
        {
            var expression = source.Body as MemberExpression;
            if (expression != null)
            {
                var member = expression.Member;
                return member.Name;
            }
            else
            {
                UnaryExpression unex = source.Body as UnaryExpression;
                if (unex != null)
                {
                    expression = unex.Operand as MemberExpression;
                    return expression.Member.Name;
                }
            }

            throw new NotSupportedException("Only MemberExpression(s) are supported.");
        }

        public static String GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> source)
        {
            MemberExpression expression = GetMemberExpression(source);
            if (!(expression.Expression is MemberExpression))
            {
                return expression.Member.Name;
            }
            IList<String> chain = new List<String>();
            do
            {
                chain.Add(expression.Member.Name);
            } while ((expression = expression.Expression as MemberExpression) != null);

            if (chain.Count == 0)
                throw new NotSupportedException("Only MemberExpression(s) are supported.");
            return chain.Reverse().Aggregate((s1, s2) => s1 + "." + s2);
        }

        public static String GetLastMemberName<T, TProperty>(this Expression<Func<T, TProperty>> source)
        {
            MemberExpression expression = GetMemberExpression(source);

            return expression.Member.Name;
        }

        public static List<String> GetListMemberName<T, TProperty>(this Expression<Func<T, TProperty>> source)
        {
            List<String> chain = new List<String>();

            MemberExpression expression = GetMemberExpression(source);
            if (!(expression.Expression is MemberExpression))
            {
                chain.Add(expression.Member.Name);
            }
            else
            {
                do
                {
                    chain.Add(expression.Member.Name);
                } while ((expression = expression.Expression as MemberExpression) != null);
            }


            if (chain.Count == 0)
                throw new NotSupportedException("Only MemberExpression(s) are supported.");
            chain.Reverse();
            return chain;
        }

        private static MemberExpression GetMemberExpression<T, TProperty>(Expression<Func<T, TProperty>> source)
        {
            var expression = source.Body as MemberExpression;
            if (expression == null)
            {
                UnaryExpression unex = source.Body as UnaryExpression;
                if (unex != null)
                {
                    expression = unex.Operand as MemberExpression;
                }
            }
            return expression;
        }
    }
}
