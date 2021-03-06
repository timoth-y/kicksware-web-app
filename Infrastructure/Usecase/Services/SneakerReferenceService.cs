﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.References;
using Core.Extension;
using Core.Gateway;
using Core.Model;
using Core.Model.Parameters;
using Core.Reference;
using Core.Repositories;
using Core.Services;

namespace Infrastructure.Usecase
{
	public class SneakerReferenceService : ISneakerReferenceService
	{
		private readonly ISneakerReferenceRepository _repository;

		private readonly IQueryBuilder _builder;

		private readonly Task<List<SneakerReference>> _cachedSneakers;

		public SneakerReferenceService(ISneakerReferenceRepository repository, IQueryBuilder builder)
		{
			_repository = repository;
			_builder = builder;
			_cachedSneakers = FetchAsync();
		}

		#region CRUD sync

		public SneakerReference FetchUnique(string sneakerId, RequestParams requestParams = default) =>
			_repository.GetUnique(sneakerId ?? string.Empty, requestParams);

		public List<SneakerReference> Fetch(RequestParams requestParams = default) => _repository.Get(requestParams);

		public List<SneakerReference> Fetch(IEnumerable<string> idList, RequestParams requestParams = default) => _repository.Get(idList, requestParams);

		public List<SneakerReference> Fetch(object queryObject, RequestParams requestParams = default) => _repository.Get(queryObject, requestParams);

		public List<SneakerReference> Fetch(RequestQuery query, RequestParams requestParams = default) => _repository.Get(query, requestParams);

		public SneakerReference Store(SneakerReference sneakerReference, RequestParams requestParams = default) => _repository.Post(sneakerReference, requestParams);

		public List<SneakerReference> Store(List<SneakerReference> sneakerReferences, RequestParams requestParams = default) => _repository.Post(sneakerReferences, requestParams);

		public bool Modify(SneakerReference sneakerReference, RequestParams requestParams = default) => _repository.Update(sneakerReference, requestParams);

		public int Count() => _repository.Count();

		public int Count(RequestQuery query, RequestParams requestParams = default) => _repository.Count(query, requestParams);

		public int Count(object queryObject, RequestParams requestParams = default) => _repository.Count(queryObject, requestParams);

		#endregion

		#region CRUD async

		public Task<SneakerReference> FetchUniqueAsync(string sneakerId, RequestParams requestParams = default) => _repository.GetUniqueAsync(sneakerId ?? string.Empty, requestParams);

		public Task<List<SneakerReference>> FetchAsync(RequestParams requestParams = default) => _repository.GetAsync(requestParams);

		public Task<List<SneakerReference>> FetchAsync(IEnumerable<string> idList, RequestParams requestParams = default) => _repository.GetAsync(idList, requestParams);

		public Task<List<SneakerReference>> FetchAsync(object queryObject, RequestParams requestParams = default) => _repository.GetAsync(queryObject, requestParams);

		public Task<List<SneakerReference>> FetchAsync(RequestQuery query, RequestParams requestParams = default) => _repository.GetAsync(query, requestParams);

		public Task<SneakerReference> StoreAsync(SneakerReference sneakerReference, RequestParams requestParams = default) => _repository.PostAsync(sneakerReference, requestParams);

		public Task<List<SneakerReference>> StoreAsync(List<SneakerReference> sneakerReferences, RequestParams requestParams = default) => _repository.PostAsync(sneakerReferences, requestParams);

		public Task<bool> ModifyAsync(SneakerReference sneakerReference, RequestParams requestParams = default) => _repository.UpdateAsync(sneakerReference, requestParams);

		public Task<int> CountAsync(RequestQuery query, RequestParams requestParams = default) => _repository.CountAsync(query, requestParams);

		public Task<int> CountAsync(object queryObject, RequestParams requestParams = default) => _repository.CountAsync(queryObject, requestParams);

		#endregion

		#region Usecases

		public List<SneakerReference> GetRelated(SneakerReference reference, RequestParams requestParams = default)
		{
			var requiredCount = requestParams?.Limit ?? 5;
			var query = _builder
				.Reset()
				.SetQueryArguments("brandname", ExpressionType.And, new FilterParameter(reference.BrandName))
				.SetQueryArguments("modelname", ExpressionType.And, new FilterParameter(reference.ModelName, ExpressionType.Regex))
				.Build();

			if (requestParams?.Limit != null) requestParams.Limit++; // later -1 current reference

			var related = Fetch(query, requestParams);
			if ((related is null || related.Count < requiredCount) && reference.BaseModel != null)
			{
				query = _builder
					.Reset()
					.SetQueryArguments("brandname", ExpressionType.And, new FilterParameter(reference.BrandName))
					.SetQueryArguments("modelname", ExpressionType.And, new FilterParameter(reference.BaseModel?.Name, ExpressionType.Regex))
					.Build();
				var lessRelated = Fetch(query, new RequestParams{Limit = 100});
				if (lessRelated != null && lessRelated.Any())
				{
					lessRelated = lessRelated.OrderBySimilarity(r => r.ModelName, reference.ModelName)
						.Where(r => r.UniqueID != reference.UniqueID)
						.ToList();
					related = (related?.Union(lessRelated) ?? lessRelated).ToList();
				}
			}

			if (related != null && related.Count < requiredCount && reference.Brand != null && _cachedSneakers.IsCompleted && _cachedSneakers.Result != null)
			{
				var brandRelated = _cachedSneakers.Result.Where(r => r.BrandName == reference.BrandName).ToList();
				if (brandRelated.Any())
				{
					brandRelated = brandRelated.OrderBySimilarity(r => r.ModelName, reference.ModelName)
						.Where(r => r.UniqueID != reference.UniqueID)
						.ToList();
					related = related.Union(brandRelated).ToList();
				}
			}

			return related?
				.Where(r => r.UniqueID != reference.UniqueID)
				.Distinct(new EntityComparer<SneakerReference>())
				.Take(requiredCount).ToList();
		}

		public async Task<List<SneakerReference>> GetRelatedAsync(SneakerReference reference, RequestParams requestParams = default)
		{
			var requiredCount = requestParams?.Limit ?? 5;
			var query = _builder
				.Reset()
				.SetQueryArguments("modelname", ExpressionType.And, new FilterParameter(reference.ModelName, ExpressionType.Regex))
				.Build();
			if (requestParams?.Limit != null) requestParams.Limit++; // later -1 current reference

			var related = await FetchAsync(query, requestParams);
			if ((related is null || related.Count < requiredCount) && reference.Model?.BaseModel != null)
			{
				query = _builder
					.Reset()
					.SetQueryArguments("modelname", ExpressionType.And, new FilterParameter(reference.ModelName, ExpressionType.Regex))
					.Build();
				var lessRelated = await FetchAsync(query);
				if (lessRelated != null && lessRelated.Any())
				{
					lessRelated = lessRelated.OrderBySimilarity(r => r.ModelName, reference.ModelName)
						.Where(r => r.UniqueID != reference.UniqueID)
						.ToList();
					related = (related?.Union(lessRelated) ?? lessRelated).ToList();
				}
			}

			if (related != null && related.Count < requiredCount && reference.Brand != null && _cachedSneakers.IsCompleted && _cachedSneakers.Result != null)
			{
				var brandRelated = _cachedSneakers.Result.Where(r => r.BrandName == reference.BrandName).ToList();
				if (brandRelated.Any())
				{
					brandRelated = brandRelated.OrderBySimilarity(r => r.ModelName, reference.ModelName)
						.Where(r => r.UniqueID != reference.UniqueID)
						.ToList();
					related = related.Union(brandRelated).ToList();
				}
			}

			return related?
				.Where(r => r.UniqueID != reference.UniqueID)
				.Distinct(new EntityComparer<SneakerReference>())
				.Take(requiredCount).ToList();
		}

		public List<SneakerReference> GetFeatured(IEnumerable<string> models, RequestParams requestParams = default)
		{
			var query = _builder
				.Reset()
				.SetQueryArguments("modelname", ExpressionType.Or, models.Select(model => new FilterParameter(model, ExpressionType.Regex)).ToArray())
				.Build();

			if (requestParams?.SortBy is null)
			{
				(requestParams ??= new RequestParams()).SortBy = "releasedate";
				requestParams.SortDirection = SortDirection.Descending;
			}
			return Fetch(query, requestParams);
		}

		public async Task<List<SneakerReference>> GetFeaturedAsync(IEnumerable<string> models, RequestParams requestParams = default)
		{
			var query = _builder
				.Reset()
				.SetQueryArguments("modelname", ExpressionType.Or, models.Select(model => new FilterParameter(model, ExpressionType.Regex)).ToArray())
				.Build();

			if (requestParams?.SortBy is null)
			{
				(requestParams ??= new RequestParams()).SortBy = "releasedate";
				requestParams.SortDirection = SortDirection.Descending;
			}
			return await FetchAsync(query, requestParams);
		}

		public List<SneakerReference> GetLatest(int limit) => Fetch(new RequestParams {Limit = limit, SortBy = "releasedate", SortDirection = SortDirection.Descending});

		public Task<List<SneakerReference>> GetLatestAsync(int limit) => FetchAsync(new RequestParams {Limit = limit, SortBy = "releasedate", SortDirection = SortDirection.Descending});

		#endregion
	}
}