export interface PagedResult<T> {
  stories: T[],
  currentPage: number,
  pageSize: number,
  totalRecord: number,
  totalPages: number
}
