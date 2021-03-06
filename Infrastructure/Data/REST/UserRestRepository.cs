﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Users;
using Core.Gateway;
using Core.Model.Parameters;
using Core.Repositories;
using Infrastructure.Gateway.REST;
using Infrastructure.Gateway.REST.ProductRequests.Sneakers;
using Infrastructure.Gateway.REST.Requests.Users;
using Infrastructure.Gateway.REST.Users;

namespace Infrastructure.Data
{
	public class UserRestRepository : IUserRepository
	{
		private IGatewayClient<IGatewayRestRequest> _client;

		public UserRestRepository(IGatewayClient<IGatewayRestRequest> client) => _client = client;

		public User GetUnique(string userId, RequestParams requestParams = default) =>
			_client.Request<User>(new GetUserRequest(userId) {RequestParams = requestParams});

		public List<User> Get(RequestParams requestParams = default) =>
			_client.Request<List<User>>(new GetAllUserRequest {RequestParams = requestParams});

		public List<User> Get(IEnumerable<string> usernames, RequestParams requestParams = default) =>
			_client.Request<List<User>>(new GetQueriedUsersRequest(usernames) {RequestParams = requestParams});

		public List<User> Get(RequestQuery query, RequestParams requestParams = default) =>
			_client.Request<List<User>>(new GetQueriedUsersRequest(query.GetQuery<Dictionary<string, object>>()) {RequestParams = requestParams});

		public List<User> Get(object queryObject, RequestParams requestParams = default) =>
			_client.Request<List<User>>(new GetQueriedUsersRequest(queryObject) {RequestParams = requestParams});

		public User Post(User user, RequestParams requestParams = default) =>
			_client.Request<User>(new PostUserRequest(user) {RequestParams = requestParams});

		public bool Update(User user, RequestParams requestParams = default) =>
			_client.Request(new PutUserRequest(user) {RequestParams = requestParams});

		public bool Delete(User user, RequestParams requestParams = default) =>
			_client.Request(new DeleteUserRequest(user) {RequestParams = requestParams});

		public bool Delete(string userId, RequestParams requestParams = default) =>
			_client.Request(new DeleteUserRequest(userId) {RequestParams = requestParams});

		public int Count(RequestQuery query, RequestParams requestParams = default) =>
			_client.Request<int>(new CountUsersRequest(query.GetQuery<Dictionary<string, object>>()) {RequestParams = requestParams});

		public int Count(object queryObject, RequestParams requestParams = default) =>
			_client.Request<int>(new CountUsersRequest(queryObject) {RequestParams = requestParams});

		public int Count() => _client.Request<int>(new CountUsersRequest());

		public Task<User> GetUniqueAsync(string userId, RequestParams requestParams = default) =>
			_client.RequestAsync<User>(new GetUserRequest(userId));

		public Task<List<User>> GetAsync(RequestParams requestParams = default) =>
			_client.RequestAsync<List<User>>(new GetAllUserRequest {RequestParams = requestParams});

		public Task<List<User>> GetAsync(IEnumerable<string> usernames, RequestParams requestParams = default) =>
			_client.RequestAsync<List<User>>(new GetQueriedUsersRequest(usernames) {RequestParams = requestParams});

		public Task<List<User>> GetAsync(RequestQuery query, RequestParams requestParams = default) =>
			_client.RequestAsync<List<User>>(new GetQueriedUsersRequest(query.GetQuery<Dictionary<string, object>>()) {RequestParams = requestParams});


		public Task<List<User>> GetAsync(object queryObject, RequestParams requestParams = default) =>
			_client.RequestAsync<List<User>>(new GetQueriedUsersRequest(queryObject) {RequestParams = requestParams});


		public Task<User> PostAsync(User user, RequestParams requestParams = default) =>
			_client.RequestAsync<User>(new PostUserRequest(user) {RequestParams = requestParams});


		public Task<bool> UpdateAsync(User user, RequestParams requestParams = default) =>
			_client.RequestAsync(new PutUserRequest(user) {RequestParams = requestParams});

		public Task<bool> DeleteAsync(User user, RequestParams requestParams = default) =>
			_client.RequestAsync(new DeleteUserRequest(user) {RequestParams = requestParams});

		public Task<bool> DeleteAsync(string userId, RequestParams requestParams = default) =>
			_client.RequestAsync(new DeleteUserRequest(userId) {RequestParams = requestParams});

		public Task<int> CountAsync(RequestQuery query, RequestParams requestParams = default) =>
			_client.RequestAsync<int>(new CountUsersRequest(query.GetQuery<Dictionary<string, object>>()) {RequestParams = requestParams});

		public Task<int> CountAsync(object queryObject, RequestParams requestParams = default) =>
			_client.RequestAsync<int>(new CountUsersRequest(queryObject) {RequestParams = requestParams});

		public Task<int> CountAsync() => _client.RequestAsync<int>(new CountUsersRequest());
		public string GetTheme(string userID) => _client.Request<string>(new GetThemeRequest(userID));

		public Task<string> GetThemeAsync(string userID) => _client.RequestAsync<string>(new GetThemeRequest(userID));
	}
}
