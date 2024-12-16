import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { StoriesService } from '../services/stories.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ng-story-app';

  constructor(private storiesService: StoriesService){}

  ngOnInit(): void {
    this.storiesService.getNewStories(1, 500).subscribe();
  }
}
