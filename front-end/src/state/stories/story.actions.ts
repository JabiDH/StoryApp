import { createAction, props } from "@ngrx/store";
import { Story } from "../../models/story.model";
import { PageData } from "../../models/page-data.model";

export const loadStories = createAction(
  '[Story] Load Stories',
  props<PageData>()
);

export const loadStoriesSuccess = createAction(
  '[Story] Load Stories Success',
  props<{ stories: Story[], totalRecords: number }>()
);

export const loadStoriesFailure = createAction(
  '[Story] Load Stories Failure',
  props<{ error: string }>()
);
