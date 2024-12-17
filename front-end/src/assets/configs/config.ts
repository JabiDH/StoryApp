import { environment } from "../../environments/environment";

export const storyApiUrls = {
  newStories: `${environment.storyAppApiUrl}/stories/new`
}

export const pagingConfigs = {
  pageSize: 10,
  pageSizeOptions: [5, 10, 25, 50, 100, 500]
}
