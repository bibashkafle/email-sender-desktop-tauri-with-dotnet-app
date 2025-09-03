import { TestBed } from '@angular/core/testing';

import { AutomationClientService } from './automation-client.service';

describe('AutomationClientService', () => {
  let service: AutomationClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AutomationClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
