export interface PagedList{
    totalDevices?: number;
    pageSize?: number;
    currentPage?: number;
    totalPages?: number;
    hasNextPage?: boolean;
    hasPreviousPage?: boolean;

}