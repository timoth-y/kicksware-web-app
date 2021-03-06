﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Users;
using Core.Gateway;

namespace Core.Services
{
	public interface IUserService : ICommonService<User>, IMailService
	{
		#region CRUD Sync

		List<User> Fetch(IEnumerable<string> usernames, RequestParams requestParams = default);

		List<User> Fetch(object queryObject, RequestParams requestParams = default);

		User Store(User user, RequestParams requestParams = default);

		bool Modify(User user, RequestParams requestParams = default);

		bool Remove(User user, RequestParams requestParams = default);

		bool Remove(string username, RequestParams requestParams = default);

		#endregion

		#region CRUD Async

		Task<List<User>> FetchAsync(IEnumerable<string> usernames, RequestParams requestParams = default);

		Task<List<User>> FetchAsync(object queryObject, RequestParams requestParams = default);

		Task<User> StoreAsync(User user, RequestParams requestParams = default);

		Task<bool> ModifyAsync(User user, RequestParams requestParams = default);

		Task<bool> RemoveAsync(User user, RequestParams requestParams = default);

		Task<bool> RemoveAsync(string username, RequestParams requestParams = default);

		#endregion

		#region Use cases

		Theme GetTheme(string userID);

		Task<Theme> GetThemeAsync(string userID);

		#endregion
	}
}