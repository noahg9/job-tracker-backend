const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface JobApplication {
  id?: number;
  company: string;
  role: string;
  status: number;
  appliedDate: string;
  notes: string;
}

export async function getAllApplications(): Promise<JobApplication[]> {
  const res = await fetch(`${API_BASE}/JobApplications`);
  return await res.json();
}

export async function addApplication(app: JobApplication): Promise<JobApplication> {
  const res = await fetch(`${API_BASE}/JobApplications`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(app),
  });
  return await res.json();
}
