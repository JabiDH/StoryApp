import { environment } from "../../environments/environment";

export const storyApiUrls = {
  getNewStories: `${environment.storyAppApiUrl}/stories/new`,
  searchNewStories: `${environment.storyAppApiUrl}/stories/new/search`
}

export const pagingConfigs = {
  pageSize: 10,
  pageSizeOptions: [5, 10, 25, 50, 100, 500]
}
