import { createReducer, on } from "@ngrx/store";
import { Story } from "../../models/story.model";
import { loadStories, loadStoriesFailure, loadStoriesSuccess } from "./story.actions";

export enum Status {
  Pending = 'pending',
  Loading = 'loading',
  Error = 'error',
  Success = 'success'
}

export interface StoryState {
  stories: Story[],
  totalRecords: number,
  error: string | null,
  status: Status
}

export const initialState: StoryState = {
  stories: [],
  totalRecords: 0,
  error: '',
  status: Status.Pending
}

export const storyReducer = createReducer(initialState,
  on(loadStories, state => ({ ...state, status: Status.Loading })),
  on(loadStoriesSuccess, (state, { stories, totalRecords }) => ({ ...state, stories: stories, totalRecords: totalRecords, error: null, status: Status.Success })),
  on(loadStoriesFailure, (state, { error }) => ({ ...state, error: error, status: Status.Error }))
);

