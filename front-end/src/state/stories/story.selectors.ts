import { createSelector } from "@ngrx/store";
import { AppState } from "../app.state";
import { StoryState } from "./story.reducer";

export const selectStoryState = (state: AppState) => state.storyState;
export const selectAllStories = createSelector(
    selectStoryState,
    (state: StoryState) => state.stories
);
export const selectTotalRecords = createSelector(
  selectStoryState,
  (state: StoryState) => state.totalRecords
);
