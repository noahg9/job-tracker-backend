import { useEffect, useState, useCallback, useMemo } from "react";
import type { FormEvent } from "react";
import {
    getAllApplications,
    addApplication,
    deleteApplication,
    updateApplication,
} from "./api";
import type { JobApplication } from "./api";
import "./App.scss";

const STATUS_OPTIONS = [
    { value: 0, label: "Applied", colorClass: "status-0" },
    { value: 1, label: "Interview", colorClass: "status-1" },
    { value: 2, label: "Offer", colorClass: "status-2" },
    { value: 3, label: "Rejected", colorClass: "status-3" },
    { value: 4, label: "Accepted", colorClass: "status-4" },
];

const SORT_OPTIONS = [
    { value: "dateAsc", label: "Applied Date ↑" },
    { value: "dateDesc", label: "Applied Date ↓" },
    { value: "companyAsc", label: "Company ↑" },
    { value: "companyDesc", label: "Company ↓" },
];

type ToastType = "success" | "error";

function App() {
    // Applications state
    const [apps, setApps] = useState<JobApplication[]>([]);
    const [loading, setLoading] = useState(false);

    // Filter & sorting (persist in localStorage)
    const [filterStatus, setFilterStatus] = useState<number | "all">(() => {
        const saved = localStorage.getItem("filterStatus");
        if (saved === null) return "all";
        return saved === "all" ? "all" : Number(saved);
    });
    const [sortOrder, setSortOrder] = useState<string>(() => {
        return localStorage.getItem("sortOrder") ?? "dateDesc";
    });

    // Form inputs for adding
    const [company, setCompany] = useState("");
    const [role, setRole] = useState("");
    const [status, setStatus] = useState(0);
    const [appliedDate, setAppliedDate] = useState(
        new Date().toISOString().substring(0, 10)
    );
    const [notes, setNotes] = useState("");

    // Editing states
    const [editingId, setEditingId] = useState<number | null>(null);
    const [editCompany, setEditCompany] = useState("");
    const [editRole, setEditRole] = useState("");
    const [editStatus, setEditStatus] = useState(0);
    const [editAppliedDate, setEditAppliedDate] = useState("");
    const [editNotes, setEditNotes] = useState("");

    // Toast notification state
    const [toast, setToast] = useState<{ message: string; type: ToastType } | null>(null);

    // Confirmation modal state
    const [confirmDeleteId, setConfirmDeleteId] = useState<number | null>(null);

    // Load apps on mount
    useEffect(() => {
        setLoading(true);
        getAllApplications()
            .then(setApps)
            .catch(() => showToast("Failed to load applications", "error"))
            .finally(() => setLoading(false));
    }, []);

    // Persist filter & sort
    useEffect(() => {
        localStorage.setItem("filterStatus", String(filterStatus));
    }, [filterStatus]);
    useEffect(() => {
        localStorage.setItem("sortOrder", sortOrder);
    }, [sortOrder]);

    // Toast helper
    const showToast = useCallback((message: string, type: ToastType = "success") => {
        setToast({ message, type });
        setTimeout(() => setToast(null), 3000);
    }, []);

    // Filtered & sorted apps memoized
    const filteredApps = useMemo(() => {
        let filtered =
            filterStatus === "all" ? apps : apps.filter((app) => app.status === filterStatus);

        switch (sortOrder) {
            case "dateAsc":
                filtered = filtered.slice().sort((a, b) => new Date(a.appliedDate).getTime() - new Date(b.appliedDate).getTime());
                break;
            case "dateDesc":
                filtered = filtered.slice().sort((a, b) => new Date(b.appliedDate).getTime() - new Date(a.appliedDate).getTime());
                break;
            case "companyAsc":
                filtered = filtered.slice().sort((a, b) => a.company.localeCompare(b.company));
                break;
            case "companyDesc":
                filtered = filtered.slice().sort((a, b) => b.company.localeCompare(a.company));
                break;
        }
        return filtered;
    }, [apps, filterStatus, sortOrder]);

    // Add Application Handler
    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        if (!company.trim() || !role.trim()) return;

        const newApp: JobApplication = {
            company: company.trim(),
            role: role.trim(),
            status,
            appliedDate: new Date(appliedDate).toISOString(),
            notes: notes.trim(),
        };

        setLoading(true);
        try {
            const saved = await addApplication(newApp);
            setApps((prev) => [...prev, saved]);
            showToast("Application added");
            // Reset form
            setCompany("");
            setRole("");
            setStatus(0);
            setAppliedDate(new Date().toISOString().substring(0, 10));
            setNotes("");
        } catch {
            showToast("Failed to add application", "error");
        } finally {
            setLoading(false);
        }
    };

    // Delete with confirmation modal
    const confirmDelete = (id: number) => setConfirmDeleteId(id);
    const cancelDelete = () => setConfirmDeleteId(null);

    const handleDelete = async () => {
        if (confirmDeleteId === null) return;
        setLoading(true);
        try {
            await deleteApplication(confirmDeleteId);
            setApps((prev) => prev.filter((app) => app.id !== confirmDeleteId));
            showToast("Application deleted");
        } catch {
            showToast("Failed to delete application", "error");
        } finally {
            setLoading(false);
            setConfirmDeleteId(null);
        }
    };

    // Start editing
    const startEditing = (app: JobApplication) => {
        setEditingId(app.id ?? null);
        setEditCompany(app.company);
        setEditRole(app.role);
        setEditStatus(app.status);
        setEditAppliedDate(app.appliedDate.substring(0, 10));
        setEditNotes(app.notes ?? "");
    };

    // Cancel editing
    const cancelEditing = () => {
        setEditingId(null);
        setEditCompany("");
        setEditRole("");
        setEditStatus(0);
        setEditAppliedDate("");
        setEditNotes("");
    };

    // Save edit
    const saveEdit = async () => {
        if (editingId === null) return;
        if (!editCompany.trim() || !editRole.trim()) return;

        const updatedApp: JobApplication = {
            id: editingId,
            company: editCompany.trim(),
            role: editRole.trim(),
            status: editStatus,
            appliedDate: new Date(editAppliedDate).toISOString(),
            notes: editNotes.trim(),
        };

        setLoading(true);
        try {
            await updateApplication(editingId, updatedApp);
            setApps((prev) =>
                prev.map((app) => (app.id === editingId ? updatedApp : app))
            );
            showToast("Application updated");
            cancelEditing();
        } catch {
            showToast("Failed to update application", "error");
        } finally {
            setLoading(false);
        }
    };

    // Status Badge Component
    function StatusBadge({ status }: { status: number }) {
        const option = STATUS_OPTIONS.find((opt) => opt.value === status);
        return (
            <span className={`status-badge ${option?.colorClass}`}>
                {option?.label}
            </span>
        );
    }

    return (
        <div className="app-container">
            <h1 className="app-title">
                Job Applications Tracker
            </h1>

            {/* Filter & Sort Controls */}
            <div className="controls">
                <label>
                    Filter by Status:
                    <select
                        value={filterStatus}
                        onChange={(e) =>
                            setFilterStatus(e.target.value === "all" ? "all" : Number(e.target.value))
                        }
                    >
                        <option value="all">All</option>
                        {STATUS_OPTIONS.map((opt) => (
                            <option key={opt.value} value={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                </label>

                <label>
                    Sort by:
                    <select
                        value={sortOrder}
                        onChange={(e) => setSortOrder(e.target.value)}
                    >
                        {SORT_OPTIONS.map((opt) => (
                            <option key={opt.value} value={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                </label>
            </div>

            {/* Add Application Form */}
            <form onSubmit={handleSubmit} className="add-application-form">
                <div>
                    <label htmlFor="company">Company *</label>
                    <input
                        id="company"
                        value={company}
                        onChange={(e) => setCompany(e.target.value)}
                        disabled={loading}
                        required
                    />
                </div>

                <div>
                    <label htmlFor="role">Role *</label>
                    <input
                        id="role"
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                        disabled={loading}
                        required
                    />
                </div>

                <div>
                    <label htmlFor="status">Status *</label>
                    <select
                        id="status"
                        value={status}
                        onChange={(e) => setStatus(Number(e.target.value))}
                        disabled={loading}
                    >
                        {STATUS_OPTIONS.map((opt) => (
                            <option key={opt.value} value={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                </div>

                <div>
                    <label htmlFor="appliedDate">Applied Date *</label>
                    <input
                        id="appliedDate"
                        type="date"
                        value={appliedDate}
                        onChange={(e) => setAppliedDate(e.target.value)}
                        disabled={loading}
                        required
                    />
                </div>

                <div className="form-notes">
                    <label htmlFor="notes">Notes</label>
                    <textarea
                        id="notes"
                        value={notes}
                        onChange={(e) => setNotes(e.target.value)}
                        disabled={loading}
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading || !company.trim() || !role.trim()}
                >
                    {loading ? "Saving..." : "Add Application"}
                </button>
            </form>

            {/* Applications List */}
            {loading && <p className="loading-message">Loading applications...</p>}

            {!loading && filteredApps.length === 0 && (
                <p className="no-applications-message">No applications found.</p>
            )}

            {!loading && filteredApps.length > 0 && (
                <ul className="applications-list">
                    {filteredApps.map((app) => (
                        <li key={app.id} className={`application-item ${editingId === app.id ? "editing" : ""}`}>
                            {/* Editing mode */}
                            {editingId === app.id ? (
                                <>
                                    <div className="edit-grid">
                                        <div>
                                            <label>Company *</label>
                                            <input
                                                value={editCompany}
                                                onChange={(e) => setEditCompany(e.target.value)}
                                                disabled={loading}
                                                required
                                            />
                                        </div>

                                        <div>
                                            <label>Role *</label>
                                            <input
                                                value={editRole}
                                                onChange={(e) => setEditRole(e.target.value)}
                                                disabled={loading}
                                                required
                                            />
                                        </div>

                                        <div>
                                            <label>Status *</label>
                                            <select
                                                value={editStatus}
                                                onChange={(e) => setEditStatus(Number(e.target.value))}
                                                disabled={loading}
                                            >
                                                {STATUS_OPTIONS.map((opt) => (
                                                    <option key={opt.value} value={opt.value}>
                                                        {opt.label}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>

                                        <div>
                                            <label>Applied Date *</label>
                                            <input
                                                type="date"
                                                value={editAppliedDate}
                                                onChange={(e) => setEditAppliedDate(e.target.value)}
                                                disabled={loading}
                                                required
                                            />
                                        </div>

                                        <div>
                                            <label>Notes</label>
                                            <textarea
                                                value={editNotes}
                                                onChange={(e) => setEditNotes(e.target.value)}
                                                disabled={loading}
                                            />
                                        </div>
                                    </div>

                                    <div className="edit-actions">
                                        <button
                                            onClick={saveEdit}
                                            disabled={loading || !editCompany.trim() || !editRole.trim()}
                                            className="save-button"
                                        >
                                            Save
                                        </button>
                                        <button
                                            onClick={cancelEditing}
                                            disabled={loading}
                                            className="cancel-button"
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </>
                            ) : (
                                <>
                                    <h2>{app.company}</h2>
                                    <p>
                                        <strong>Role:</strong> {app.role}
                                    </p>
                                    <p>
                                        <strong>Status:</strong> <StatusBadge status={app.status} />
                                    </p>
                                    <p>
                                        <strong>Applied Date:</strong>{" "}
                                        {new Date(app.appliedDate).toLocaleDateString()}
                                    </p>
                                    {app.notes && (
                                        <p className="notes-section">
                                            <strong>Notes:</strong> {app.notes}
                                        </p>
                                    )}

                                    <div className="actions-container">
                                        <button
                                            onClick={() => startEditing(app)}
                                            disabled={loading}
                                            className="edit-button"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => confirmDelete(app.id!)}
                                            disabled={loading}
                                            className="delete-button"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </>
                            )}
                        </li>
                    ))}
                </ul>
            )}

            {/* Confirmation Modal */}
            {confirmDeleteId !== null && (
                <div
                    role="dialog"
                    aria-modal="true"
                    aria-labelledby="confirm-dialog-title"
                    className="confirmation-modal-overlay"
                >
                    <div className="confirmation-modal-content">
                        <h3 id="confirm-dialog-title">
                            Confirm Delete
                        </h3>
                        <p>Are you sure you want to delete this application?</p>
                        <div className="modal-actions">
                            <button
                                onClick={cancelDelete}
                                disabled={loading}
                                className="cancel-button"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleDelete}
                                disabled={loading}
                                className="confirm-delete-button"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Toast Notification */}
            {toast && (
                <div
                    role="alert"
                    aria-live="assertive"
                    className={`toast-notification ${toast.type}`}
                >
                    {toast.message}
                </div>
            )}
        </div>
    );
}

export default App;