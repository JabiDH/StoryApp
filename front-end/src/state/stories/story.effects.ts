import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { AppState } from "../app.state";
import { StoriesService } from "../../services/stories.service";
import { Store } from "@ngrx/store";
import { loadStories, loadStoriesFailure, loadStoriesSuccess } from "./story.actions";
import { catchError, from, map, of, switchMap } from "rxjs";
import { PagedResult } from "../../models/paged-result.model";
import { Story } from "../../models/story.model";
import { PageData } from "../../models/page-data.model";
import { Action } from "rxjs/internal/scheduler/Action";

@Injectable()
export class StoryEffects {
  constructor(
    private actions$: Actions,
    private store: Store<AppState>,
    private storiesService: StoriesService
  ) { }

  loadStories$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadStories),
      switchMap((pageData) => {
        const getStories = pageData.searchValue ? this.storiesService.searchNewStories(pageData.pageIndex, pageData.pageSize, pageData.searchValue) :
          this.storiesService.getNewStories(pageData.pageIndex, pageData.pageSize);
        return from(getStories.pipe(
          map((res: PagedResult<Story>) => loadStoriesSuccess({ stories: res.stories, totalRecords: res.totalRecords })),
          catchError((error) => of(loadStoriesFailure({ error: error }))))
        )
      }
      )
    )
  );

}
