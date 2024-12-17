export interface PagedResult<T> {
  stories: T[],
  currentPage: number,
  pageSize: number,
  totalRecords: number,
  totalPages: number
}
