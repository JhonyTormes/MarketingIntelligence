import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LinkShortenerService } from '../../services/link-shortener';

@Component({
  selector: 'app-create-link',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-link.html',
  styleUrl: './create-link.css',
})
export class CreateLink {
  originalUrl: string = '';
  shortenedUrl: string = '';
  isLoading: boolean = false;
  copySuccess: boolean = false;

  constructor(private linkService: LinkShortenerService) { }

  shorten() {
    if (!this.originalUrl) return;

    this.isLoading = true;
    this.linkService.shortenUrl(this.originalUrl).subscribe({
      next: (res) => {
        // Construct full URL (assuming current host or configured domain)
        // For now, using window.location.origin + /api/links/~/{shortCode} ? No, backend redirects.
        // Backend returns "ShortCode".
        // If we want to use the short domain, we need to know it.
        // For dev, it's localhost:5278/api/links/~/{shortCode}
        // But the requirement says "RedirectToOriginal" is at api/links/~/{shortCode}

        // Wait, if I'm on mobile, I want the public URL.
        // The backend returns what?
        // Let's check the service. It returns ShortenedLink { shortCode, originalUrl }.

        // Use relative path for now, or just the code. 
        // Let's construct a full URL for display.
        const baseUrl = window.location.origin.replace('4200', '5278'); // Hack for dev: frontend on 4200, backend 5278
        // Actually, via proxy it is /api/links/~/{code}
        this.shortenedUrl = `${window.location.origin}/api/links/~/${res.shortCode}`;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  copyToClipboard() {
    navigator.clipboard.writeText(this.shortenedUrl).then(() => {
      this.copySuccess = true;
      setTimeout(() => this.copySuccess = false, 2000);
    });
  }
}
