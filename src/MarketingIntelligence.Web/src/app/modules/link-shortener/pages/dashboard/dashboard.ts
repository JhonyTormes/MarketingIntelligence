import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LinkShortenerService, LinkStats, LinkClick } from '../../services/link-shortener';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  shortCodeInput: string = '';
  stats: LinkStats | null = null;
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(private linkService: LinkShortenerService) { }

  loadStats() {
    if (!this.shortCodeInput) return;

    this.isLoading = true;
    this.errorMessage = '';
    this.stats = null;

    this.linkService.getStats(this.shortCodeInput).subscribe({
      next: (data) => {
        this.stats = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Link not found or error loading stats.';
        this.isLoading = false;
      }
    });
  }
}
