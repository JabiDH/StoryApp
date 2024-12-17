import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StoriesComponent } from './stories.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { Story } from '../../models/story.model';
import { of, throwError } from 'rxjs';
import { StoriesService } from '../../services/stories.service';
import { PagedResult } from '../../models/paged-result.model';
import { PageEvent } from '@angular/material/paginator';

describe('StoriesComponent', () => {
  let component: StoriesComponent;
  let fixture: ComponentFixture<StoriesComponent>;
  let storiesService: jasmine.SpyObj<StoriesService>;

  beforeEach(async () => {
    const storiesServiceSpy = jasmine.createSpyObj('StoriesService', ['getNewStories']);
    storiesServiceSpy.getNewStories.and.returnValue(of({ stories: [ { id: 1, title: 'Story 1', url: 'http://story.com/1' } ], totalRecords: 1, currentPage: 1, pageSize: 1, totalPages: 1 } as PagedResult<Story>));

    await TestBed.configureTestingModule({
      imports: [
        StoriesComponent,
        HttpClientTestingModule
      ],
      providers: [
        provideNoopAnimations(),
        { provide: StoriesService, useValue: storiesServiceSpy }
      ]
    })
    .compileComponents();

    storiesService = TestBed.inject(StoriesService) as jasmine.SpyObj<StoriesService>;
    fixture = TestBed.createComponent(StoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load data on init', () => {
    const mockStories = [
      { id: 1, title: 'Story 1', url: 'http://story.com/1' },
      { id: 2, title: 'Story 2', url: 'http://story.com/2' },
      { id: 3, title: 'Story 3', url: 'http://story.com/3' }
    ];
    storiesService.getNewStories.and.returnValue(of({ stories: mockStories, totalRecords: 1, currentPage: 1, pageSize: 1, totalPages: 1 } as PagedResult<Story>));

    component.ngOnInit();

    component.stories$.subscribe(stories => {
      expect(stories).toEqual(mockStories);
      expect(component.storiesDataSource.data).toEqual(mockStories);
    });
  });

  it('should handle empty stories response', () => {
    const emptyResponse = { stories: [], totalRecords: 0, currentPage: 0, pageSize: 0, totalPages: 0 } as PagedResult<Story>;
    storiesService.getNewStories.and.returnValue(of(emptyResponse));

    component.ngOnInit();

    component.stories$.subscribe(stories => {
      expect(stories.length).toBe(0);
      expect(component.storiesDataSource.data).toEqual([]);
    });
  });

  it('should call getNewStories with correct parameters', () => {
    const spy = spyOn(component, 'getNewStories').and.callThrough();
    component.getNewStories(1, 10);

    expect(spy).toHaveBeenCalledWith(1, 10);
  });

  it('should change page on page change', () => {
    const pageEvent = { pageIndex: 1, pageSize: 10 }
    component.onPageChange(pageEvent as PageEvent);

    expect(component.pageIndex).toBe(1);
    expect(component.pageSize).toBe(10);
    expect(storiesService.getNewStories).toHaveBeenCalledWith(2, 10);
  });

  it('should apply filter', () => {
    const inputEvent = { target: { value: 'story' } };
    component.applyFilter(inputEvent as any);

    expect(component.storiesDataSource.filter).toBe('story');
  });

  it('should handle error in loadData and show an error message', () => {
        const errorResponse = new Error('Error fetching stories');
        storiesService.getNewStories.and.returnValue(throwError(() => errorResponse));

        component.ngOnInit();

        component.stories$.subscribe(stories => {
          expect(stories).toEqual([]);
        });
      });
});
