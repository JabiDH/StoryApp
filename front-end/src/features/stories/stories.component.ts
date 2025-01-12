import { Component, OnInit } from '@angular/core';
import { StoriesService } from '../../services/stories.service';
import { PagedResult } from '../../models/paged-result.model';
import { Story } from '../../models/story.model';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { pagingConfigs } from '../../assets/configs/config';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AsyncPipe, CommonModule } from '@angular/common';
import { BehaviorSubject, Observable, of, switchMap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { loadStories } from '../../state/stories/story.actions';
import { PageData } from '../../models/page-data.model';
import { selectAllStories, selectTotalRecords } from '../../state/stories/story.selectors';
import { AppState } from '../../state/app.state';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    AsyncPipe,
    FormsModule
  ],
  templateUrl: './stories.component.html',
  styleUrl: './stories.component.css'
})
export class StoriesComponent implements OnInit {
  title: string = 'Stories';
  displayedColumns: string[] = ['title', 'url'];
  searchValue: string = '';

  pageIndex: number = 1;
  pageSize: number = pagingConfigs.pageSize;
  pageSizeOptions: number[] = pagingConfigs.pageSizeOptions;

  totalRecords$!: Observable<number>;
  stories$!: Observable<any>;

  constructor(
    private store: Store<AppState>
  ) { }

  ngOnInit(): void {
    this.stories$ = this.store.select(selectAllStories);
    this.totalRecords$ = this.store.select(selectTotalRecords);

    this.loadData();
  }

  loadData() {
    const pageData = { pageIndex: this.pageIndex, pageSize: this.pageSize, searchValue: this.searchValue } as PageData
    this.store.dispatch(loadStories(pageData));
  }

  searchData() {
    this.pageIndex = 1;
    this.loadData();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

}
