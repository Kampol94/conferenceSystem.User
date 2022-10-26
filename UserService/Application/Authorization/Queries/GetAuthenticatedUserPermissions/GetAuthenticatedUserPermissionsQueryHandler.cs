﻿using Dapper;
using UserService.Application.Authorization.GetUserPermissions;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Queries;

namespace UserService.Application.Authorization.GetAuthenticatedUserPermissions;

internal class GetAuthenticatedUserPermissionsQueryHandler : IQueryHandler<GetAuthenticatedUserPermissionsQuery, List<UserPermissionDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    private readonly IExecutionContextAccessor _executionContextAccessor;

    public GetAuthenticatedUserPermissionsQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        IExecutionContextAccessor executionContextAccessor)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<List<UserPermissionDto>> Handle(GetAuthenticatedUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        if (!_executionContextAccessor.IsAvailable)
        {
            return new List<UserPermissionDto>();
        }

        var connection = _sqlConnectionFactory.GetOpenConnection();

        const string sql = "SELECT " +
                           "[UserPermission].[PermissionCode] AS [Code] " +
                           "FROM [users].[v_UserPermissions] AS [UserPermission] " +
                           "WHERE [UserPermission].UserId = @UserId";
        var permissions = await connection.QueryAsync<UserPermissionDto>(
            sql,
            new { _executionContextAccessor.UserId });

        return permissions.AsList();
    }
}