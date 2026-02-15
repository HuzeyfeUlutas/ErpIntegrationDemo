export interface PagedResult<T> {
  result: T;
  rowCount: number;
  pageIndex: number;
  pageSize: number;
}

export enum Campus {
  Istanbul = 'Istanbul',
  Ankara = 'Ankara',
  Izmir = 'Izmir',
}

export enum Title {
  Engineer = 'Engineer',
  Supervisor = 'Supervisor',
  Manager = 'Manager',
  Technician = 'Technician',
  Emergency = 'Emergency'
}

// ─── Label Maps ────────────────────────────────────────

export const CAMPUS_OPTIONS = [
  { label: 'İstanbul', value: Campus.Istanbul },
  { label: 'Ankara', value: Campus.Ankara },
  { label: 'İzmir', value: Campus.Izmir },
] as const;

export const CAMPUS_LABEL: Record<Campus, string> = {
  [Campus.Istanbul]: 'İstanbul',
  [Campus.Ankara]: 'Ankara',
  [Campus.Izmir]: 'İzmir',
};

export const TITLE_OPTIONS = [
  { label: 'Mühendis', value: Title.Engineer },
  { label: 'Supervisor', value: Title.Supervisor },
  { label: 'Müdür', value: Title.Manager },
  { label: 'Teknisyen', value: Title.Technician },
  { label: 'Acil Durum Çalışanı', value: Title.Emergency },
] as const;

export const TITLE_LABEL: Record<Title, string> = {
  [Title.Engineer]: 'Mühendis',
  [Title.Supervisor]: 'Supervisor',
  [Title.Manager]: 'Müdür',
  [Title.Technician]: 'Teknisyen',
  [Title.Emergency]: 'Acil Durum Çalışanı',
};